import ENDPOINT_URLS from "../constants/endpoints";

const AdminService = {
  async getDashboard(userToken) {
    return await fetch(ENDPOINT_URLS.GET_DASHBOARD, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async deleteUser(userId, userToken) {
    return await fetch(ENDPOINT_URLS.DELETE_USER(userId), {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async updateUserRoles(updateRolesRequest, userToken) {
    return await fetch(`${ENDPOINT_URLS.UPDATE_USER_ROLES}`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
      body: JSON.stringify(updateRolesRequest),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.log(error));
  },

  async getRolesForEmail(email, userToken) {
    const params = { email: email };
    const queryParams = new URLSearchParams(params);
    const constructedEndpoint = `${
      ENDPOINT_URLS.GET_ROLES_FOR_EMAIL
    }?${queryParams.toString()}`;

    return await fetch(constructedEndpoint, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },
};

export default AdminService;
