using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

using System.Linq.Expressions;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

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
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.ChatUsers.Count == Ids.Length && c.ChatUsers.All(cu => Ids.Contains(cu.UserId)), cancellationToken);
        }

        public Task<PagedResult<ChatDto>> GetChatsForUserAsync(Guid userId, PageRequest page, CancellationToken cancellationToken)
        {
            return appDbContext.Chats.AsNoTracking()
                .AsSplitQuery()
                .Where(chat => chat.ChatUsers.Any(member => member.UserId == userId))
                .Select(ChatProjection(1))
                .OrderByDescending(chat => chat.UpdatedAt).ThenByDescending(chat => chat.Id)
                .ToPageAsync(page, cancellationToken);
        }

        public async Task<ChatDto?> GetChatDetailsAsync(Guid chatId, CancellationToken cancellationToken)
        {
            var chat = await appDbContext.Chats.AsNoTracking().AsSplitQuery()
                .Where(chat => chat.Id == chatId)
                .Select(ChatProjection(PageRequest.DefaultPageSize))
                .SingleOrDefaultAsync(cancellationToken);
            // Select the newest bounded window in SQL; present it in reading order.
            chat?.Messages.Reverse();
            return chat;
        }

        private static Expression<Func<Chat, ChatDto>> ChatProjection(int messageLimit) => chat => new ChatDto
        {
            Id = chat.Id, CreatedAt = chat.CreatedAt, UpdatedAt = chat.UpdatedAt,
            MessageCount = chat.Messages.Count(),
            Users = chat.ChatUsers.OrderBy(member => member.UserId).Select(member => new UserDto
            {
                Id = member.UserId, Email = member.User.Email!,
                FirstName = member.User.FirstName!, LastName = member.User.LastName!
            }).ToList(),
            Messages = chat.Messages.OrderByDescending(message => message.CreatedAt).ThenByDescending(message => message.Id)
                .Take(messageLimit).Select(message => new MessageDto
                {
                    Id = message.Id, ChatId = message.ChatId, SenderId = message.SenderId,
                    Content = message.Content, CreatedAt = message.CreatedAt
                }).ToList()
        };

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
