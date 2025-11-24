using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/messaging")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessagingService messagingService;

        public MessageController(IMessagingService messagingService)
        {
            this.messagingService = messagingService;
        }

        [HttpGet]
        [Route("chats")]
        public async Task<ActionResult<List<ChatDto>>> GetChats(CancellationToken cancellationToken = default)
        {
            var result = await messagingService.GetUserChatsAsync(cancellationToken);
            
            return Ok(result);
        }

        [HttpPost]
        [Route("chats/{id::guid}")]
        public async Task<ActionResult<ChatDto>> StartChat(Guid otherUserId, CancellationToken cancellationToken = default)
        {
            var result = await messagingService.StartChatAsync(otherUserId, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<MessageDto>> SendMessageAsync([FromBody] MessageDto messageDto, CancellationToken cancellationToken = default)
        {
            var result = await messagingService.SendMessageAsync(messageDto, cancellationToken);
            
            return Ok(result);
        }
    }
}
