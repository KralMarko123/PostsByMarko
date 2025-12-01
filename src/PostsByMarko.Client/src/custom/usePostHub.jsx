import { React, useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT_URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const usePostHub = () => {
  const { user } = useAuth();
  const [signalR, setSignalR] = useState({
    connection: new HubConnectionBuilder()
      .withUrl(ENDPOINT_URLS.POST_HUB, {
        accessTokenFactory: () => user.token,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect([0, 2, 4, 6, 8, 10, 15, 20, 30])
      .build(),

    lastMessageRegistered: "",
  });

  const PostEvents = {
    onPostCreated: (post) =>
      setSignalR({
        ...signalR,
        lastMessageRegistered: `New Post created at ${post.createdAt}`,
      }),
    onPostUpdated: (post) =>
      setSignalR({
        ...signalR,
        lastMessageRegistered: `Post updated at ${post.lastUpdatedAt}`,
      }),
    onPostDeleted: (postId) =>
      setSignalR({
        ...signalR,
        lastMessageRegistered: `Post with Id '${postId}' was deleted`,
      }),
  };

  useEffect(() => {
    if (signalR.connection) {
      signalR.connection
        .start()
        .then(() => {
          signalR.connection.on("PostCreated", (post) =>
            PostEvents.onPostCreated(post)
          );

          signalR.connection.on("PostUpdated", (post) =>
            PostEvents.onPostUpdated(post)
          );

          signalR.connection.on("PostDeleted", (postId) =>
            PostEvents.onPostDeleted(postId)
          );
        })
        .catch((error) =>
          console.error(`SignalR connection	failed with error: ${error}`)
        );
    }
  }, [signalR.connection]);

  return signalR;
};
