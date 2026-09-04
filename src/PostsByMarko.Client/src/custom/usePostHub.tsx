import { useEffect, Dispatch, useRef } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { createHubConnection } from "./useSignalRConnection";
import { HubConnection } from "@microsoft/signalr";
import { PostChangeNotification } from "@typeConfigs/post";
import { PostHubEvents } from "../types/signalr";
import { AppAction } from "@typeConfigs/context";

export const usePostHub = (
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

    const connection: HubConnection = createHubConnection(ENDPOINT_URLS.POST_HUB, token);

    connectionRef.current = connection;

    if (connection) {
      connection
        .start()
        .then(() => {
          connection.on(PostHubEvents.PostCreated, (notification: PostChangeNotification) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `Post '${notification.id}' created at ${notification.occurredAt}`,
            })
          );

          connection.on(PostHubEvents.PostUpdated, (notification: PostChangeNotification) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `Post '${notification.id}' updated at ${notification.occurredAt}`,
            })
          );

          connection.on(PostHubEvents.PostDeleted, (postId: string) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `Post with Id '${postId}' was deleted`,
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
