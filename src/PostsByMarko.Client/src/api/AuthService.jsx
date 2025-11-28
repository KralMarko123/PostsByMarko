import ENDPOINT_URLS from "../constants/endpoints";

const AuthService = {
  async login(loginRequest) {
    return await fetch(ENDPOINT_URLS.LOGIN, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(loginRequest),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async register(registerRequest) {
    return await fetch(ENDPOINT_URLS.REGISTER, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(registerRequest),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },
};

export default AuthService;
