import { useEffect, Dispatch, useRef } from "react";
import { HubConnection } from "@microsoft/signalr";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { createHubConnection } from "./useSignalRConnection";
import { MessageHubEvents } from "../types/signalr";
import { Chat, Message } from "@typeConfigs/messaging";
import { AppAction } from "@typeConfigs/context";

export const useMessageHub = (
  token: string | null | undefined,
  dispatch: Dispatch<AppAction>
) => {
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    if (!token) {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }

      return;
    }

    if (connectionRef.current) return;

    const connection: HubConnection = createHubConnection(
      ENDPOINT_URLS.MESSAGE_HUB,
      token
    );

    connectionRef.current = connection;

    if (connection) {
      connection
        .start()
        .then(() => {
          connection.on(MessageHubEvents.MessageSent, (message: Message) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `New Message sent at ${message.createdAt}`,
            })
          );

          connection.on(MessageHubEvents.ChatCreated, (chat: Chat) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `Chat started at ${chat.createdAt}`,
            })
          );
        })
        .catch((error) => console.error(`SignalR connection	failed with error: ${error}`));
    }

    return () => {
      connection.stop();
      connectionRef.current = null;
    };
  }, [token, dispatch]);
};
