using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("IntegrationCollection")]
    public class UserControllerTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private readonly string controllerPrefix = "/api/user";

        public UserControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);
        }

        public Task DisposeAsync() => Task.CompletedTask;


        [Fact]
        public async Task should_return_all_users()
        {
            // Arrange
            var mapper = postsByMarkoApiFactory.Resolve<IMapper>();
            var userManager = postsByMarkoApiFactory.Resolve<UserManager<User>>();

            var allUsers = await userManager.Users.ToListAsync();
            var userDtos = mapper.Map<List<UserDto>>(allUsers);

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/all");
            var responseContent = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserDto>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            users.Should().NotBeNullOrEmpty();
            users.Count.Should().Be(userDtos.Count);

            foreach (var userDto in userDtos)
            {
                users.Should().ContainEquivalentOf(userDto);
            }
        }

        [Fact]
        public async Task should_return_filtered_users()
        {
            // Arrange
            var mapper = postsByMarkoApiFactory.Resolve<IMapper>();
            var userManager = postsByMarkoApiFactory.Resolve<UserManager<User>>();

            var allUsers = await userManager.Users.ToListAsync();
            var testUser = allUsers.Find(u => u.Email == TestingConstants.TEST_USER_EMAIL);
            var userDtos = mapper.Map<List<UserDto>>(allUsers);
            var filteredUserDto = userDtos.First(u => u.Id == testUser.Id);

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/all?exceptId={testUser.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var filteredUsers = JsonConvert.DeserializeObject<List<UserDto>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            filteredUsers.Should().NotBeNullOrEmpty();
            filteredUsers.Count.Should().Be(userDtos.Count - 1);
            filteredUsers.Should().NotContainEquivalentOf(filteredUserDto);
        }


        [Fact]
        public async Task should_return_user()
        {
            // Arrange
            var mapper = postsByMarkoApiFactory.Resolve<IMapper>();
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var userToReturn = mapper.Map<UserDto>(testUser);

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/{testUser.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            user.Should().NotBeNull();
            user.Should().BeEquivalentTo(userToReturn);
        }
    }
}