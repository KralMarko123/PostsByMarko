using aspnetserver.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetserver.Data.Repos.Posts
{
    internal static class PostsRepository
    {
        private static AppDbContext appDbContext;

        public static async Task<List<Post>> GetPostsAsync()
        {
            return await appDbContext.Posts.ToListAsync();
        }

        public static async Task<Post> GetPostByIdAsync(int postId)
        {
            return await appDbContext.Posts.FirstOrDefaultAsync(p => p.PostId.Equals(postId));
        }

        public static async Task<bool> CreatePostAsync(Post postToCreate)
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


        public static async Task<bool> UpdatePostAsync(Post postToUpdate)
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

        public static async Task<bool> DeletePostAsync(int postId)
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
