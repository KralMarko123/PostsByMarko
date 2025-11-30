import ENDPOINT_URLS from "../constants/endpoints";
import ApiClient from "./ApiClient";

export const AdminService = {
  getDashboard: async (userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_DASHBOARD, {
      method: "GET",
      token: userToken,
    }),

  deleteUser: async (userId, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.DELETE_USER(userId), {
      method: "DELETE",
      token: userToken,
    }),

  updateUserRoles: async (updateRolesRequest, userToken) =>
    await ApiClient.apiRequest(`${ENDPOINT_URLS.UPDATE_USER_ROLES}`, {
      method: "PUT",
      body: updateRolesRequest,
      token: userToken,
    }),

  getRolesForEmail: async (email, userToken) => {
    const params = { email: email };
    const queryParams = new URLSearchParams(params);
    const constructedEndpoint = `${
      ENDPOINT_URLS.GET_ROLES_FOR_EMAIL
    }?${queryParams.toString()}`;

    return await ApiClient.apiRequest(constructedEndpoint, {
      method: "GET",
      token: userToken,
    });
  },
};
