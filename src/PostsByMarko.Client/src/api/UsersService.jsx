import ENDPOINT__URLS from "../constants/endpoints";

const UsersService = {
  async getAllUsers(userToken) {
    return await fetch(ENDPOINT__URLS.GET_ALL_USERS, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then((response) => response.json())
      .then((requestResult) => requestResult.payload)
      .catch((error) => console.log(error));
  },

  async getUserById(userId, userToken) {
    return await fetch(`${ENDPOINT__URLS.GET_USER_BY_ID}/${userId}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then((response) => response.json())
      .then((requestResult) => requestResult)
      .catch((error) => console.log(error));
  },

  async GetAdminDashboard(userToken) {
    return await fetch(ENDPOINT__URLS.GET_ADMIN_DASHBOARD, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then((response) => response.json())
      .then((requestResult) => requestResult)
      .catch((error) => console.log(error));
  },
};

export default UsersService;
