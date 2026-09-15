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

        public async Task<Chat?> GetChatByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.Id == Id, cancellationToken);
        }

        public async Task<Chat?> GetChatByUserIdsAsync(Guid[] Ids, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .AsSplitQuery()
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.ChatUsers.Count == Ids.Length && c.ChatUsers.All(cu => Ids.Contains(cu.UserId)), cancellationToken);
        }

        public async Task<List<Chat>> GetChatsForUserAsync(User user, CancellationToken cancellationToken)
        {
            return await appDbContext.Chats
                .AsSplitQuery()
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .Where(c => c.ChatUsers.Any(cu => cu.UserId == user.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Chat> GetOrCreateChatAsync(Chat chat, CancellationToken cancellationToken)
        {
            // Serialize creation for the pair across API instances, using existing user rows as locks.
            // ReadCommitted ensures the second request sees the chat committed by the first request.
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            var userIds = chat.ChatUsers.Select(member => member.UserId).OrderBy(id => id).ToArray();
            foreach (var userId in userIds)
            {
                await appDbContext.Users
                    .FromSqlInterpolated($"SELECT * FROM AspNetUsers WHERE Id = {userId} FOR UPDATE")
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }

            var existing = await GetChatByUserIdsAsync(userIds, cancellationToken);
            if (existing is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return existing;
            }

            await appDbContext.Chats.AddAsync(chat, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return chat;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
