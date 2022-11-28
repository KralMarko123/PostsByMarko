using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Users;
using Microsoft.EntityFrameworkCore;

namespace aspnetserver.Data.Repos.Posts
{
    public class PostsRepository : IPostsRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly IUsersRepository usersRepository;

        public PostsRepository(AppDbContext appDbContext, IUsersRepository usersRepository)
        {
            this.appDbContext = appDbContext;
            this.usersRepository = usersRepository;
        }

        public async Task<List<Post>> GetPostsAsync()
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
                postToCreate.CreatedDate = DateTime.UtcNow;
                postToCreate.LastUpdatedDate = postToCreate.CreatedDate;

                await appDbContext.Posts.AddAsync(postToCreate);

                return await appDbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<bool> UpdatePostAsync(Post updatedPost)
        {

            try
            {
                var postToUpdate = await GetPostByIdAsync(updatedPost.PostId);

                postToUpdate.LastUpdatedDate = DateTime.UtcNow;
                postToUpdate.Title = updatedPost.Title;
                postToUpdate.Content = updatedPost.Content;

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
