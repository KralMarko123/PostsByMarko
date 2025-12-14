import { HttpMethod } from "constants/enums";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { ApiClient } from "./ApiClient";
import { AdminDashboardResponse, UpdateUserRolesRequest } from "types/admin";

export const AdminService = {
  getDashboard: async (userToken: string): Promise<AdminDashboardResponse[]> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_DASHBOARD, {
      method: HttpMethod.GET,
      token: userToken,
    }),

  deleteUser: async (userId: string, userToken: string): Promise<unknown> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.DELETE_USER(userId), {
      method: HttpMethod.DELETE,
      token: userToken,
    }),

  updateUserRoles: async (request: UpdateUserRolesRequest, userToken: string): Promise<string[]> =>
    await ApiClient.apiRequest(`${ENDPOINT_URLS.UPDATE_USER_ROLES}`, {
      method: HttpMethod.PUT,
      body: request,
      token: userToken,
    }),

  getRolesForEmail: async (email: string, userToken: string): Promise<string[]> => {
    const params = { email: email };
    const queryParams = new URLSearchParams(params);
    const constructedEndpoint = `${ENDPOINT_URLS.GET_ROLES_FOR_EMAIL}?${queryParams.toString()}`;

    return await ApiClient.apiRequest(constructedEndpoint, {
      method: HttpMethod.GET,
      token: userToken,
    });
  },
};
