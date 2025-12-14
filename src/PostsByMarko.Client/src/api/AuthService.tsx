import { AuthUser, LoginRequest, RegisterRequest } from "types/auth";
import { ApiClient } from "./ApiClient";
import { HttpMethod } from "constants/enums";
import { ENDPOINT_URLS } from "constants/endpoints";

export const AuthService = {
  login: async (request: LoginRequest): Promise<AuthUser> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.LOGIN, {
      method: HttpMethod.POST,
      body: request,
    }),

  register: async (request: RegisterRequest): Promise<unknown> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.REGISTER, {
      method: HttpMethod.POST,
      body: request,
    }),

  validate: async (userToken: string): Promise<AuthUser> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.VALIDATE, {
      method: HttpMethod.GET,
      token: userToken,
    }),
};
