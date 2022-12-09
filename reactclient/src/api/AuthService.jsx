import ENDPOINT__URLS from "../constants/endpoints";

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
			else return response.text();
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
		}).then((response) => {
			if (response.ok) return response.status;
			else return response.text();
		});
	},
};

export default AuthService;
