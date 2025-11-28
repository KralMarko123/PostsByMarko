import ENDPOINT_URLS from "../constants/endpoints";

const UserService = {
  async getUsers(userToken) {
    return await fetch(ENDPOINT_URLS.GET_USERS, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async getUserById(userId, userToken) {
    return await fetch(ENDPOINT_URLS.GET_USER(userId), {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },
};

export default UserService;
