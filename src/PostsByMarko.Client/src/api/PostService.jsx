import ENDPOINT_URLS from "../constants/endpoints";

const PostService = {
  async getPosts(userToken) {
    return await fetch(ENDPOINT_URLS.GET_POSTS, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async getPostById(postId, userToken) {
    return await fetch(ENDPOINT_URLS.GET_POST_BY_ID(postId), {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async createPost(createPostRequest, userToken) {
    return await fetch(ENDPOINT_URLS.CREATE_POST, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${userToken}`,
      },
      body: JSON.stringify(createPostRequest),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async updatePost(postId, updatePostRequest, userToken) {
    return await fetch(ENDPOINT_URLS.UPDATE_POST(postId), {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${userToken}`,
      },
      body: JSON.stringify(updatePostRequest),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async deletePostById(postId, userToken) {
    return await fetch(ENDPOINT_URLS.DELETE_POST(postId), {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },
};

export default PostService;
