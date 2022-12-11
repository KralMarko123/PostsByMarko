using System.Dynamic;

namespace PostsTesting.Utility.Builders
{
    public class ObjectBuilder
    {
        private readonly dynamic Object;

        public ObjectBuilder()
        {
            Object = new ExpandoObject();
        }

        public ObjectBuilder WithId(string id)
        {
            Object.Id = id;
            return this;
        }

        public ObjectBuilder WithPostId(string postId)
        {
            Object.PostId = postId;
            return this;
        }

        public ObjectBuilder WithTitle(string title)
        {
            Object.Title = title;
            return this;
        }

        public ObjectBuilder WithContent(string content)
        {
            Object.Content = content;
            return this;
        }

        public ExpandoObject Build()
        {
            return Object;
        }
    }
}
