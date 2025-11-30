import ENDPOINT_URLS from "../constants/endpoints";
import ApiClient from "./ApiClient";

export const PostService = {
  getPosts: async (userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_POSTS, {
      method: "GET",
      token: userToken,
    }),

  getPostById: async (postId, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_POST_BY_ID(postId), {
      method: "GET",
      token: userToken,
    }),

  createPost: async (createPostRequest, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.CREATE_POST, {
      method: "POST",
      body: createPostRequest,
      token: userToken,
    }),

  updatePost: async (postId, updatePostRequest, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.UPDATE_POST(postId), {
      method: "PUT",
      body: updatePostRequest,
      token: userToken,
    }),

  deletePostById: async (postId, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.DELETE_POST(postId), {
      method: "DELETE",
      token: userToken,
    }),
};
