using aspnetserver.Builders;
using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;

namespace aspnetserver.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IPostsRepository postsRepository;

        public UsersService(IUsersRepository usersRepository, IPostsRepository postsRepository)
        {
            this.usersRepository = usersRepository;
            this.postsRepository = postsRepository;
        }

        public async Task<RequestResult> GetAllUsernamesAsync()
        {
            var allUsernames = await usersRepository.GetAllUsernamesAsync();
            return new RequestResultBuilder().Ok().WithPayload(allUsernames).Build();
        }
    }
}
