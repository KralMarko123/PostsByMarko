import { useEffect, Dispatch, useRef } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { createHubConnection } from "./useSignalRConnection";
import { HubConnection } from "@microsoft/signalr";
import { Post } from "@typeConfigs/post";
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
          connection.on(PostHubEvents.PostCreated, (post: Post) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `New Post created at ${post.createdAt}`,
            })
          );

          connection.on(PostHubEvents.PostUpdated, (post: Post) =>
            dispatch({
              type: "MESSAGE_REGISTERED",
              message: `Post updated at ${post.lastUpdatedAt}`,
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
