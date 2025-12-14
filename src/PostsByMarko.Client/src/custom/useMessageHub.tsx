import { useState, useEffect } from "react";
import { HubConnection } from "@microsoft/signalr";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { useAuth } from "./useAuth";
import { createHubConnection } from "./useSignalRConnection";
import { MessageHubEvents } from "../types/signalr";
import { Chat, Message } from "@typeConfigs/messaging";

export const useMessageHub = () => {
  const { user } = useAuth();
  const connection: HubConnection = createHubConnection(
    ENDPOINT_URLS.POST_HUB,
    user?.token!
  );
  const [lastMessageRegistered, setLastMessageRegistered] = useState<string>("");

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          connection.on(MessageHubEvents.MessageSent, (message: Message) =>
            setLastMessageRegistered(`New Message sent at ${message.createdAt}`)
          );

          connection.on(MessageHubEvents.ChatCreated, (chat: Chat) =>
            setLastMessageRegistered(`Chat started at ${chat.createdAt}`)
          );
        })
        .catch((error) => console.error(`SignalR connection	failed with error: ${error}`));
    }
  }, [connection]);

  return lastMessageRegistered;
};
