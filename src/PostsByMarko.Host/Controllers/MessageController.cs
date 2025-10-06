using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
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

        [HttpPost]
        [Route("/sendMessage")]
        [Tags("Messaging Endpoints")]
        public async Task<RequestResult> SendMessageAsync([FromBody] MessageDto messageDto)
        {
            LoadRequestClaims();
            return await messagingService.SendMessageAsync(messageDto);
        }
    }
}
