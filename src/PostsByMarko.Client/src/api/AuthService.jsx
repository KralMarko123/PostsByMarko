import ENDPOINT_URLS from "../constants/endpoints";
import ApiClient from "./ApiClient";

export const AuthService = {
  login: async (loginRequest) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.LOGIN, {
      method: "POST",
      body: loginRequest,
    }),

  register: async (registerRequest) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.REGISTER, {
      method: "POST",
      body: registerRequest,
    }),

  validate: async (userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.VALIDATE, {
      method: "GET",
      token: userToken,
    }),
};
