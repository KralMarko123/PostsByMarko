using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;

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
        [Route("chats/user/{otherUserId:guid}")]
        public async Task<ActionResult<ChatDto>> StartChat(Guid otherUserId, CancellationToken cancellationToken = default)
        {
            var result = await messagingService.StartChatAsync(otherUserId, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageRequest request, CancellationToken cancellationToken = default)
        {
            var result = await messagingService.SendMessageAsync(request, cancellationToken);
            
            return Ok(result);
        }
    }
}
