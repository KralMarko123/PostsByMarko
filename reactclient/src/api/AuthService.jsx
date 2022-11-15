import ENDPOINT__URLS from "../constants/endpoints";
import { LOGIN_ERROR, REGISTER_ERROR } from "../constants/exceptions";

const AuthService = {
	async login(userToLogin) {
		return await fetch(ENDPOINT__URLS.LOGIN, {
			method: "POST",
			headers: {
				"Access-Control-Allow-Origin": "*",
				"Content-Type": "application/json",
			},
			body: JSON.stringify(userToLogin),
		}).then((response) => {
			if (response.ok) return response.json();
			else if (response.status === 401) {
				throw new LOGIN_ERROR("Unauthorized");
			}
		});
	},

	async register(userToRegister) {
		return await fetch(ENDPOINT__URLS.REGISTER, {
			method: "POST",
			headers: {
				"Access-Control-Allow-Origin": "*",
				"Content-Type": "application/json",
			},
			body: JSON.stringify({ ...userToRegister, email: userToRegister.username }),
		})
			.then((response) => {
				if (response.ok) return true;
				else if (response.status === 400) return response.json();
			})
			.then((responseObject) => {
				if (responseObject.errors) {
					responseObject.errors.forEach((error) => {
						if (error.code.includes("Duplicate")) {
							throw new REGISTER_ERROR("Duplicate Username");
						}
					});
				}
				return responseObject;
			});
	},
};

export default AuthService;
