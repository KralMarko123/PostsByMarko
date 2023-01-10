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

			if (requestResult.statusCode === 200) return requestResult.payload;
			else throw new Error(requestResult.message);
		});
	},

	async register(userToRegister) {
		return await fetch(ENDPOINT__URLS.REGISTER, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify({ ...userToRegister, email: userToRegister.username }),
		}).then(async (response) => {
			const requestResult = await response.json();

			if (requestResult.statusCode === 201) return;
			else throw new Error(requestResult.message);
		});
	},
};

export default AuthService;
