using PostsByMarko.Host.Application.Helper;
﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Posts;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.Host.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly IMapper mapper;
        private readonly IHubContext<PostHub, IPostClient> postHub;

        public PostService(IPostRepository postRepository, IUserRepository userRepository,
            ICurrentRequestAccessor currentRequestAccessor, IMapper mapper, IHubContext<PostHub, IPostClient> postHub)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.currentRequestAccessor = currentRequestAccessor;
            this.mapper = mapper;
            this.postHub = postHub;
        }

        public async Task<List<PostDto>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {currentUserId} was not found");
            var userRoles = await userRepository.GetRolesForUserAsync(currentUser);
            var allPosts = await postRepository.GetPostsAsync(cancellationToken);

            if (!userRoles.Contains(RoleConstants.ADMIN))
            {
                allPosts.RemoveAll(p => p.Hidden && p.AuthorId != currentUser.Id);
            }

            return mapper.Map<List<PostDto>>(allPosts);
        }

        public async Task<PostDto> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {currentUserId} was not found");
            var userRoles = await userRepository.GetRolesForUserAsync(currentUser);
            var post = await postRepository.GetPostByIdAsync(Id, cancellationToken) ?? throw new ResourceNotFoundException($"Post with Id: {Id} was not found");
            
            if (post.Hidden && !userRoles.Contains(RoleConstants.ADMIN) && post.AuthorId != currentUser.Id)
            {
                throw new ForbiddenException("You are not authorized to view this post");
            } 

            return mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {currentUserId} was not found");

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            {
                throw new BadRequestException("Post title and content cannot be empty");
            }
           
            var post = mapper.Map<Post>(request);

            post.CreatedAt = DateTime.UtcNow;
            post.AuthorId = currentUser.Id;
            post.Author = currentUser;

            post = await postRepository.AddPostAsync(post, cancellationToken);

            await postRepository.SaveChangesAsync(cancellationToken);

            var postDto = mapper.Map<PostDto>(post);

            await NotificationDelivery.SendAsync(() => postHub.Clients.All.PostCreated(new PostChangeDto(postDto.Id, postDto.CreatedAt)));

            return postDto;
        }

        public async Task<PostDto> UpdatePostAsync(Guid Id, UpdatePostRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {currentUserId} was not found");
            var userRoles = await userRepository.GetRolesForUserAsync(currentUser);
            var post = await postRepository.GetPostByIdAsync(Id, cancellationToken) ?? throw new ResourceNotFoundException($"Post with Id: {Id} was not found");

            if (currentUser.Id != post.AuthorId && !userRoles.Contains(RoleConstants.ADMIN))
            {
                throw new ForbiddenException("You are not authorized to update this post");
            }

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
                throw new BadRequestException("Post title and content cannot be empty");

            post.Title = request.Title;
            post.Content = request.Content;
            post.Hidden = request.Hidden;
            post.LastUpdatedAt = DateTime.UtcNow;

            await postRepository.UpdatePostAsync(post);
            await postRepository.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<PostDto>(post);

            await NotificationDelivery.SendAsync(() => postHub.Clients.All.PostUpdated(new PostChangeDto(result.Id, result.LastUpdatedAt)));

            return result;
        }

        public async Task DeletePostByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {currentUserId} was not found");
            var userRoles = await userRepository.GetRolesForUserAsync(currentUser);
            var post = await postRepository.GetPostByIdAsync(Id, cancellationToken) ?? throw new ResourceNotFoundException($"Post with Id: {Id} was not found");

            if (currentUser.Id != post.AuthorId && !userRoles.Contains(RoleConstants.ADMIN))
            {
                throw new ForbiddenException("You are not authorized to delete this post");
            }
            
            await postRepository.DeletePostAsync(post);
            await postRepository.SaveChangesAsync(cancellationToken);
            await NotificationDelivery.SendAsync(() => postHub.Clients.All.PostDeleted(Id));
        }
    }
}
