import ENDPOINT_URLS from "../constants/endpoints";

const MessagingService = {
  async getChats(userToken) {
    return await fetch(ENDPOINT_URLS.GET_CHATS, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async startChat(otherUserId, userToken) {
    return await fetch(ENDPOINT_URLS.START_CHAT(otherUserId), {
      method: "POST",
      headers: {
        Authorization: `Bearer ${userToken}`,
        "Content-Type": "application/json",
      },
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },

  async sendMessage(messageToSend, userToken) {
    return await fetch(ENDPOINT_URLS.SEND_MESSAGE, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${userToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(messageToSend),
    })
      .then(async (response) => await response.json())
      .catch((error) => console.error(error));
  },
};

export default MessagingService;
