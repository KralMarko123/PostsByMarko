import { React, useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT_URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const useMessageHub = () => {
  const { user } = useAuth();
  const [signalR, setSignalR] = useState({
    connection: new HubConnectionBuilder()
      .withUrl(ENDPOINT_URLS.MESSAGE_HUB, {
        accessTokenFactory: () => user.token,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect([0, 2, 4, 6, 8, 10, 15, 20, 30])
      .build(),

    lastMessageRegistered: "",
  });

  const MessagingEvents = {
    onMessageSent: (message) =>
      setSignalR({
        ...signalR,
        lastMessageRegistered: `New Message sent at ${message.createdAt}`,
      }),
    onChatCreated: (chat) =>
      setSignalR({
        ...signalR,
        lastMessageRegistered: `New Chat started at ${chat.createdAt}`,
      }),
  };

  useEffect(() => {
    if (signalR.connection) {
      signalR.connection
        .start()
        .then(() => {
          signalR.connection.on("MessageSent", (message) =>
            MessagingEvents.onMessageSent(message)
          );

          signalR.connection.on("ChatCreated", (chat) => {
            MessagingEvents.onChatCreated(chat);
          });
        })
        .catch((error) =>
          console.error(`SignalR connection	failed with error: ${error}`)
        );
    }
  }, [signalR.connection]);

  return signalR;
};
