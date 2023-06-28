using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Data.Repos.Posts
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
            await appDbContext.Posts.AddAsync(postToCreate);

            var postAddedSuccessfully = await appDbContext.SaveChangesAsync() >= 1;

            if (postAddedSuccessfully) return await GetPostByIdAsync(postToCreate.Id);
            else throw new Exception("Error during post creation");
        }


        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            appDbContext.Posts.Update(postToUpdate);

            var postUpdatedSuccessfully = await appDbContext.SaveChangesAsync() >= 1;

            if (postUpdatedSuccessfully) return true;
            else return false;
        }

        public async Task<bool> DeletePostAsync(Post postToDelete)
        {
            appDbContext.Remove(postToDelete);

            var postDeletedSuccessfully = await appDbContext.SaveChangesAsync() >= 1;

            if (postDeletedSuccessfully) return true;
            else return false;
        }
    }
}
