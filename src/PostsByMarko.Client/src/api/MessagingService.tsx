import { Chat, Message } from "types/messaging";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { ApiClient } from "./ApiClient";
import { HttpMethod } from "constants/enums";

export const MessagingService = {
  getChats: async (userToken: string): Promise<Chat[]> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.GET_CHATS, {
      method: HttpMethod.GET,
      token: userToken,
    }),

  startChat: async (otherUserId: string, userToken: string): Promise<Chat> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.START_CHAT(otherUserId), {
      method: HttpMethod.POST,
      token: userToken,
    }),

  sendMessage: async (messageToSend: Message, userToken: string): Promise<Message> =>
    await ApiClient.apiRequest(ENDPOINT_URLS.SEND_MESSAGE, {
      method: HttpMethod.POST,
      token: userToken,
      body: messageToSend,
    }),
};
