import ENDPOINT__URLS from "../constants/endpoints";
import { LOGIN_ERROR } from "../constants/exceptions";

const AuthService = {
	async login(userToLogin) {
		return await fetch(ENDPOINT__URLS.LOGIN, {
			method: "POST",
			headers: {
				"Access-Control-Allow-Origin": "*",
				"Content-Type": "application/json",
			},
			body: JSON.stringify(userToLogin),
		})
			.then((response) => {
				if (response.ok) return response.json();
				else throw new LOGIN_ERROR("Error during login.");
			})
			.then((responseFromServer) => responseFromServer);
	},
};

export default AuthService;
