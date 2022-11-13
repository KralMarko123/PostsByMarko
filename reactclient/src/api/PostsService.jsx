import ENDPOINT__URLS from "../constants/endpoints";
import {
	CREATE_POST_ERROR,
	DELETE_POST_ERROR,
	POST_NOT_FOUND_ERROR,
	UPDATE_POST_ERROR,
} from "../constants/exceptions";

const PostsService = {
	async getAllPosts(userToken) {
		return await fetch(ENDPOINT__URLS.GET_ALL_POSTS, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		}).then((response) => {
			if (response.ok) return response.json();
			else throw new Error("Posts not found.");
		});
	},

	async getPostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.GET_POST_BY_ID}/${postId}`, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		})
			.then((response) => {
				if (response.ok) return response.json();
				else throw new POST_NOT_FOUND_ERROR(`Post with id: ${postId} was not found.`);
			})
			.then((postFromServer) => postFromServer);
	},

	async createPost(postToCreate, userToken) {
		return await fetch(ENDPOINT__URLS.CREATE_POST, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
			body: JSON.stringify(postToCreate),
		})
			.then((response) => {
				if (response.ok) return true;
				CREATE_POST_ERROR("Error during post creation.");
			})
			.then((responseFromServer) => responseFromServer);
	},

	async updatePost(postToUpdate, userToken) {
		return await fetch(ENDPOINT__URLS.UPDATE_POST, {
			method: "PUT",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${userToken}`,
			},
			body: JSON.stringify(postToUpdate),
		})
			.then((response) => {
				if (response.ok) return true;
				else throw new UPDATE_POST_ERROR(`Error during post update.`);
			})
			.then((responseFromServer) => responseFromServer);
	},

	async deletePostById(postId, userToken) {
		return await fetch(`${ENDPOINT__URLS.DELETE_POST_BY_ID}/${postId}`, {
			method: "DELETE",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		})
			.then((response) => {
				if (response.ok) return true;
				else throw new DELETE_POST_ERROR(`Error during post deletion.`);
			})
			.then((responseFromServer) => responseFromServer);
	},
};

export default PostsService;
