import ENDPOINT_URLS from "../constants/endpoints";
import ApiClient from "./ApiClient";

export const UserService = {
  getUsers: async (userToken, exceptId = null) => {
    let constructedEndpoint = ENDPOINT_URLS.GET_USERS;

    if (exceptId !== null) {
      const params = { exceptId: exceptId };
      const queryParams = new URLSearchParams(params);
      constructedEndpoint = `${
        ENDPOINT_URLS.GET_USERS
      }?${queryParams.toString()}`;
    }

    return await ApiClient.apiRequest(constructedEndpoint, {
      method: "GET",
      token: userToken,
    });
  },

  getUserById: async (userId, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_USER(userId), {
      method: "GET",
      token: userToken,
    }),
};
