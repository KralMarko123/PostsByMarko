import ENDPOINT_URLS from "../constants/endpoints";
import ApiClient from "./ApiClient";

export const MessagingService = {
  getChats: async (userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_CHATS, {
      method: "GET",
      token: userToken,
    }),

  startChat: async (otherUserId, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.START_CHAT(otherUserId), {
      method: "POST",
      token: userToken,
    }),

  sendMessage: async (messageToSend, userToken) =>
    await ApiClient.apiRequest(ENDPOINT_URLS.SEND_MESSAGE, {
      method: "POST",
      token: userToken,
      body: messageToSend,
    }),
};
