import { CreatePostRequest, Post, UpdatePostRequest } from "types/post";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { ApiClient } from "./ApiClient";
import { HttpMethod } from "constants/enums";

export const PostService = {
  getPosts: async (userToken: string): Promise<Post[]> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_POSTS, {
      method: HttpMethod.GET,
      token: userToken,
    }),

  getPostById: async (postId: string, userToken: string): Promise<Post> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_POST_BY_ID(postId), {
      method: HttpMethod.GET,
      token: userToken,
    }),

  createPost: async (request: CreatePostRequest, userToken: string): Promise<Post> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.CREATE_POST, {
      method: HttpMethod.POST,
      body: request,
      token: userToken,
    }),

  updatePost: async (postId: string, request: UpdatePostRequest, userToken: string): Promise<Post> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.UPDATE_POST(postId), {
      method: HttpMethod.PUT,
      body: request,
      token: userToken,
    }),

  deletePostById: async (postId: string, userToken: string): Promise<unknown> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.DELETE_POST(postId), {
      method: HttpMethod.DELETE,
      token: userToken,
    }),
};
