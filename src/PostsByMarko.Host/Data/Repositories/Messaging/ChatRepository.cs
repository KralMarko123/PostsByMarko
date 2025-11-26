using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext appDbContext;
        
        public ChatRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Chat?> GetChatByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Chat?> GetChatByUserIdsAsync(Guid[] userIds, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.ChatUsers.Count == userIds.Length && c.ChatUsers.All(cu => userIds.Contains(cu.UserId)), cancellationToken);
        }

        public async Task<List<Chat>> GetChatsForUserAsync(User user, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .Where(c => c.ChatUsers.Any(cu => cu.UserId == user.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Chat> AddChatAsync(Chat chat, CancellationToken cancellationToken)
        {
            var result = await appDbContext.Chats.AddAsync(chat, cancellationToken);
            
            return result.Entity;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
