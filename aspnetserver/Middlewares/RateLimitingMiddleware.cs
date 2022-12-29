using aspnetserver.Decorators;
using aspnetserver.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace aspnetserver.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;

        public RateLimitingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            // read the LimitRequest attribute from the endpoint
            var rateLimitDecorator = endpoint?.Metadata.GetMetadata<LimitRequest>();
            if (rateLimitDecorator is null)
            {
                await next(context);
                return;
            }

            var key = GenerateClientKey(context);
            var clientStatistics = GetClientStatisticsByKey(key).Result;

            // Check whether the request violates the rate limit policy

            if (clientStatistics != null && DateTime.Now < clientStatistics.LastSuccessfulResponseTime.AddSeconds(rateLimitDecorator.TimeWindow)
                && clientStatistics.NumberOfRequestsCompletedSuccessfully == rateLimitDecorator.MaxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await UpdateClientStatisticsAsync(key, rateLimitDecorator.MaxRequests);
            await next(context);
        }

        private static string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
        private async Task<ClientStatistics> GetClientStatisticsByKey(string key) => await cache.GetCachedValueAsync<ClientStatistics>(key);
        private async Task UpdateClientStatisticsAsync(string key, int maxRequests)
        {
            var clientStats = cache.GetCachedValueAsync<ClientStatistics>(key).Result;

            if (clientStats != null)
            {
                clientStats.LastSuccessfulResponseTime = DateTime.UtcNow;

                if (clientStats.NumberOfRequestsCompletedSuccessfully == maxRequests) clientStats.NumberOfRequestsCompletedSuccessfully = 1;
                else clientStats.NumberOfRequestsCompletedSuccessfully++;

                await cache.SetCachedValueAsync(key, clientStats);
            }
            else
            {
                clientStats = new ClientStatistics
                {
                    LastSuccessfulResponseTime = DateTime.UtcNow,
                    NumberOfRequestsCompletedSuccessfully = 1
                };

                await cache.SetCachedValueAsync(key, clientStats);
            }
        }
    }
}
