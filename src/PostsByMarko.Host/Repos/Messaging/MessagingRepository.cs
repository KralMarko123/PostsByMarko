using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Repos.Messaging
{
    public class MessagingRepository : IMessagingRepository
    {
        private readonly AppDbContext appDbContext;

        public MessagingRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Chat> GetChatByIdAsync(int id)
        {
            return await appDbContext.Chats.FindAsync(id);
        }

        public async Task<Chat> GetChatByParticipantIdsAsync(string[] ids)
        {
            return await appDbContext.Chats.FirstOrDefaultAsync(c => c.ParticipantIds.Count == ids.Length && !c.ParticipantIds.Except(ids).Any());
        }

        public async Task<Chat> CreateChatAsync(Chat chat)
        {
            var result = await appDbContext.Chats.AddAsync(chat);
            var chatAddedSuccessfully = await appDbContext.SaveChangesAsync() >= 1;

            if (chatAddedSuccessfully) return result.Entity;
            else throw new Exception("Error during chat creation");
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            var result = await appDbContext.Messages.AddAsync(message);
            var messageAddedSuccessfully = await appDbContext.SaveChangesAsync() >= 1;

            if (messageAddedSuccessfully) return result.Entity;
            else throw new Exception("Error during message creation");
        }

        public async Task<bool> DeleteMessageAsync(Message message)
        {
            appDbContext.Messages.Remove(message);

            return await appDbContext.SaveChangesAsync() >= 1;
        }
    }
}
