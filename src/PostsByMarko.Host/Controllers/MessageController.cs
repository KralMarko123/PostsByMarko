using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Services;

namespace PostsByMarko.Host.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly IMessagingService messagingService;

        public MessageController(IMessagingService messagingService) : base()
        {
            this.messagingService = messagingService;
        }

        [HttpPost]
        [Route("/startChat")]
        [Tags("Messaging Endpoints")]
        public async Task<RequestResult> StartChatAsync([FromBody] string[] participantIds)
        {
            LoadRequestClaims();
            return await messagingService.StartChatAsync(participantIds);
        }
    }
}
