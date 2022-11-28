using aspnetserver.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetserver.Data.Repos.Posts
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

        public async Task<List<Post>> GetUserPostsAsync()
        {
            return await appDbContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await appDbContext.Posts.FirstOrDefaultAsync(p => p.PostId.Equals(postId));
        }

        public async Task<bool> CreatePostAsync(Post postToCreate)
        {
            try
            {
                await appDbContext.Posts.AddAsync(postToCreate);

                return await appDbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {

            try
            {
                appDbContext.Posts.Update(postToUpdate);

                return await appDbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeletePostAsync(int postId)
        {

            try
            {
                Post postToDelete = await GetPostByIdAsync(postId);
                appDbContext.Remove(postToDelete);

                return await appDbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
