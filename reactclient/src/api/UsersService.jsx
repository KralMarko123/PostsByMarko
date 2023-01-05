import ENDPOINT__URLS from "../constants/endpoints";

const UsersService = {
	async getAllUsers(userToken) {
		return await fetch(ENDPOINT__URLS.GET_ALL_USERS, {
			method: "GET",
			headers: {
				Authorization: `Bearer ${userToken}`,
			},
		})
			.then((response) => response.json())
			.catch((error) => console.log(error));
	},
};

export default UsersService;
