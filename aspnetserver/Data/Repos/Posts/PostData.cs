using aspnetserver.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace aspnetserver.Data.Repos.Posts
{
    public class PostData : IEntityTypeConfiguration<Post>
    {
       void IEntityTypeConfiguration<Post>.Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasData(

              new Post
              {
                  PostId = 1,
                  Title = "Post 1",
                  Content = "Post No.1's content."
              },

              new Post
              {
                  PostId = 2,
                  Title = "Post 2",
                  Content = "Post No.2's content."
              },

              new Post
              {
                  PostId = 3,
                  Title = "Post 3",
                  Content = "Post No.3's content."

              });
        }
    }
}