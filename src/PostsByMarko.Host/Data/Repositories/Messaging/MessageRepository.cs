using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public MessageRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Message?> GetMessageByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await appDbContext.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == Id, cancellationToken);
        }

        public async Task<List<Message>> GetMessagesByChatIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await appDbContext.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Where(m => m.ChatId == Id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
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
