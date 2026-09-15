using Serilog;

namespace PostsByMarko.Host.Application.Helper;

public static class NotificationDelivery
{
    // Persistence has already succeeded. A live notification failure must not ask the caller to repeat the write.
    public static async Task SendAsync(Func<Task> send)
    {
        try
        {
            await send();
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "A committed change could not be broadcast to connected clients");
        }
    }
}
