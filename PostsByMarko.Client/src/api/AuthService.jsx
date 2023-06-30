import ENDPOINT__URLS from "../constants/endpoints";

const AuthService = {
	async login(userToLogin) {
		return await fetch(ENDPOINT__URLS.LOGIN, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(userToLogin),
		}).then(async (response) => {
			const requestResult = await response.json();
			return requestResult;
		});
	},

	async register(userToRegister) {
		return await fetch(ENDPOINT__URLS.REGISTER, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(userToRegister),
		}).then(async (response) => {
			const requestResult = await response.json();
			return requestResult;
		});
	},
};

export default AuthService;
