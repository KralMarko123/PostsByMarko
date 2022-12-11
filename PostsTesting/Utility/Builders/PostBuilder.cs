using aspnetserver.Data.Models;


namespace PostsTesting.Utility.Builders
{
    public class PostBuilder
    {
        private Post post; 

        public PostBuilder()
        {
            post = new Post();
        }

        public void WithId(string id)
        {
            post.PostId = Int32.Parse(id);
        }

        public void WithTitle(string title)
        {
            post.Title = title;
        }

        public void WithContent(string content)
        {
            post.Content = content;
        }

        public Post Build()
        {
            return post;
        }
    }
}
