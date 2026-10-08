using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Posts
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext appDbContext;

        public PostRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task<PagedResult<Post>> GetPostsAsync(PageRequest page, Guid viewerId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            return appDbContext.Posts
                .Where(post => isAdmin || !post.Hidden || post.AuthorId == viewerId)
                .Include(p => p.Author)
                .AsNoTracking()
                .OrderByDescending(post => post.CreatedAt)
                .ThenByDescending(post => post.Id)
                .ToPageAsync(page, cancellationToken);
        }

        public async Task<Post?> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await appDbContext.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == Id, cancellationToken);
        }

        public async Task<Post> AddPostAsync(Post postToCreate, CancellationToken cancellationToken = default)
        {
            var result = await appDbContext.Posts.AddAsync(postToCreate, cancellationToken);
            
            return result.Entity;
        }

        public async Task UpdatePostAsync(Post postToUpdate)
        {
            appDbContext.Posts.Update(postToUpdate);
            
            await Task.CompletedTask;
        }

        public async Task DeletePostAsync(Post postToDelete)
        {
            appDbContext.Posts.Remove(postToDelete);

            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
