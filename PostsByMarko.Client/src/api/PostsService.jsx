import ENDPOINT__URLS from "../constants/endpoints";

const PostsService = {
	async getAllPosts(userToken) {
		return await fetch(ENDPOINT__URLS.GET_ALL_POSTS, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			const requestResult = response.json();
			return requestResult;
		});
	},

	async getPostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.GET_POST_BY_ID}/${postId}`, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			const requestResult = await response.json();

			if (requestResult.statusCode === 200) return requestResult.payload;
			else throw new Error(requestResult.message);
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
			const requestResult = await response.json();
			return requestResult;
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
			const requestResult = await response.json();

			if (requestResult.statusCode === 200)
				return { isSuccessful: true, message: requestResult.message };
			else throw new Error(requestResult.message);
		});
	},

	async deletePostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.DELETE_POST_BY_ID}/${postId}`, {
			method: "DELETE",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then(async (response) => {
			const requestResult = await response.json();

			if (requestResult.statusCode === 200)
				return { isSuccessful: true, message: requestResult.message };
			else throw new Error(requestResult.message);
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
			const requestResult = await response.json();

			if (requestResult.statusCode !== 200) throw new Error(requestResult.message);
		});
	},

	async toggleUserForPost(postId, username, userToken) {
		return await fetch(`${ENDPOINT__URLS.TOGGLE_USER_FOR_POST}/${postId}`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
			body: JSON.stringify(username),
		}).then(async (response) => {
			const requestResult = await response.json();

			if (requestResult.statusCode === 200)
				return { isSuccessful: true, message: requestResult.message };
			else throw new Error(requestResult.message);
		});
	},
};

export default PostsService;
