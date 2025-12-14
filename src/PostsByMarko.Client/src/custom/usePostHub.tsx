import { useState, useEffect } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { useAuth } from "./useAuth";
import { createHubConnection } from "./useSignalRConnection";
import { HubConnection } from "@microsoft/signalr";
import { Post } from "@typeConfigs/post";
import { PostHubEvents } from "../types/signalr";

export const usePostHub = () => {
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
          connection.on(PostHubEvents.PostCreated, (post: Post) =>
            setLastMessageRegistered(`New Post created at ${post.createdAt}`)
          );

          connection.on(PostHubEvents.PostUpdated, (post: Post) =>
            setLastMessageRegistered(`Post updated at ${post.lastUpdatedAt}`)
          );

          connection.on(PostHubEvents.PostDeleted, (postId: string) =>
            setLastMessageRegistered(`Post with Id '${postId}' was deleted`)
          );
        })
        .catch((error) => console.error(`SignalR connection	failed with error: ${error}`));
    }
  }, [connection]);

  return lastMessageRegistered;
};
