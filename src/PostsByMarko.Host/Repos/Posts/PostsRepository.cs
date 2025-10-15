using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Repos.Posts
{
    public class PostsRepository : IPostsRepository
    {
        private readonly AppDbContext appDbContext;

        public PostsRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await appDbContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(string postId)
        {
            return await appDbContext.Posts.FirstOrDefaultAsync(p => p.Id.Equals(postId));
        }

        public async Task<Post> CreatePostAsync(Post postToCreate)
        {
            var result = await appDbContext.Posts.AddAsync(postToCreate);
            
            await appDbContext.SaveChangesAsync();

            return result.Entity;
        }


        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            appDbContext.Posts.Update(postToUpdate);

            return await appDbContext.SaveChangesAsync() >= 1;
        }

        public async Task<bool> DeletePostAsync(Post postToDelete)
        {
            appDbContext.Remove(postToDelete);

            return await appDbContext.SaveChangesAsync() >= 1;
        }
    }
}
