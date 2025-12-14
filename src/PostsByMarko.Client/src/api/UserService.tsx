import { User } from "types/user";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { ApiClient } from "./ApiClient";
import { HttpMethod } from "constants/enums";

export const UserService = {
  getUsers: async (userToken: string, exceptId: string | null = null): Promise<User[]> => {
    let constructedEndpoint = ENDPOINT_URLS.GET_USERS;

    if (exceptId !== null) {
      const params = { exceptId: exceptId };
      const queryParams = new URLSearchParams(params);
      constructedEndpoint = `${ENDPOINT_URLS.GET_USERS}?${queryParams.toString()}`;
    }

    return await ApiClient.apiRequest(constructedEndpoint, {
      method: HttpMethod.GET,
      token: userToken,
    });
  },

  getUserById: async (userId: string, userToken: string): Promise<User> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_USER(userId), {
      method: HttpMethod.GET,
      token: userToken,
    }),
};
