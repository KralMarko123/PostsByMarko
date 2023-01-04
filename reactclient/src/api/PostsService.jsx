import ENDPOINT__URLS from "../constants/endpoints";

const PostsService = {
	async getAllPosts(userToken) {
		return await fetch(ENDPOINT__URLS.GET_ALL_POSTS, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		})
			.then((response) => response.json())
			.catch((error) => console.log(error));
	},

	async getPostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.GET_POST_BY_ID}/${postId}`, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			if (response.ok) return response.json();
			else {
				let errorMessage = await response.text();
				throw new Error(errorMessage);
			}
		});
	},

	async createPost(postToCreate, userToken) {
		return await fetch(ENDPOINT__URLS.CREATE_POST, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
			body: JSON.stringify(postToCreate),
		}).then(async (response) => {
			let responseMessage = await response.text();

			if (response.ok) return { isSuccessful: true, message: responseMessage };
			else throw new Error(responseMessage);
		});
	},

	async updatePost(postToUpdate, userToken) {
		return await fetch(ENDPOINT__URLS.UPDATE_POST, {
			method: "PUT",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
			body: JSON.stringify(postToUpdate),
		}).then(async (response) => {
			let responseMessage = await response.text();

			if (response.ok) return { isSuccessful: true, message: responseMessage };
			else throw new Error(responseMessage);
		});
	},

	async deletePostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.DELETE_POST_BY_ID}/${postId}`, {
			method: "DELETE",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			let responseMessage = await response.text();

			if (response.ok) return { isSuccessful: true, message: responseMessage };
			else throw new Error(responseMessage);
		});
	},

	async togglePostVisibility(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.TOGGLE_POST_HIDDEN}/${postId}`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			let responseMessage = await response.text();
			if (!response.ok) throw new Error(responseMessage);
		});
	},
};

export default PostsService;
