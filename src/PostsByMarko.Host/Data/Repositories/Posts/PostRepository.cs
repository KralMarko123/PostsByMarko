using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Posts
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext appDbContext;

        public PostRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Post>> GetPostsAsync(CancellationToken cancellationToken = default)
        {
            var result = await appDbContext.Posts
                .Include(p => p.Author)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return result;
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
