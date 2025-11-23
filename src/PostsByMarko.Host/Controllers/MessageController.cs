using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Services;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/message")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessagingService messagingService;

        public MessageController(IMessagingService messagingService)
        {
            this.messagingService = messagingService;
        }

        [HttpGet]
        [Route("/getChats")]
        public async Task<RequestResult> GetChatsAsync()
        {
            LoadRequestClaims();
            return await messagingService.GetUserChatsAsync(user.Id);
        }

        [HttpPost]
        [Route("/startChat")]
        public async Task<RequestResult> StartChatAsync([FromBody] string[] participantIds)
        {
            LoadRequestClaims();
            return await messagingService.StartChatAsync(participantIds);
        }

        [HttpPost]
        [Route("/sendMessage")]
        public async Task<RequestResult> SendMessageAsync([FromBody] MessageDto messageDto)
        {
            LoadRequestClaims();
            return await messagingService.SendMessageAsync(messageDto);
        }
    }
}
