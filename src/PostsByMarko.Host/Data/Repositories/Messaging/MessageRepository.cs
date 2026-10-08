using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public MessageRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<PagedResult<MessageDto>> GetMessagesAsync(Guid chatId, PageRequest page, CancellationToken cancellationToken = default)
        {
            var result = await appDbContext.Messages.AsNoTracking()
                .Where(message => message.ChatId == chatId)
                .Select(message => new MessageDto
                {
                    Id = message.Id, ChatId = message.ChatId, SenderId = message.SenderId,
                    Content = message.Content, CreatedAt = message.CreatedAt
                })
                .OrderByDescending(message => message.CreatedAt).ThenByDescending(message => message.Id)
                .ToPageAsync(page, cancellationToken);
            
            result.Items.Reverse();
            
            return result;
        }

        public async Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            var result = await appDbContext.Messages.AddAsync(message, cancellationToken);

            return result.Entity;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
