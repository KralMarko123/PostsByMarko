import { React, useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT_URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const useAdminHub = () => {
  const { user } = useAuth();
  const [signalR, setSignalR] = useState({
    connection: new HubConnectionBuilder()
      .withUrl(ENDPOINT_URLS.ADMIN_HUB, {
        accessTokenFactory: () => user.token,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect([0, 2, 4, 6, 8, 10, 15, 20, 30])
      .build(),

    lastAdminAction: "",
  });

  const AdminEvents = {
    onUpdatedUserRoles: (userId, updatedAt) =>
      setSignalR({
        ...signalR,
        lastAdminAction: `Updated roles for user with Id '${userId}' at ${updatedAt}`,
      }),
    onDeletedUser: (userId, deletedAt) =>
      setSignalR({
        ...signalR,
        lastAdminAction: `Deleted user with Id '${userId}' at ${deletedAt}`,
      }),
  };

  useEffect(() => {
    if (signalR.connection) {
      signalR.connection
        .start()
        .then(() => {
          signalR.connection.on("UpdatedUserRoles", (userId, updatedAt) =>
            AdminEvents.onUpdatedUserRoles(userId, updatedAt)
          );

          signalR.connection.on("DeletedUser", (userId, deletedAt) =>
            AdminEvents.onDeletedUser(userId, deletedAt)
          );
        })
        .catch((error) =>
          console.error(`SignalR connection	failed with error: ${error}`)
        );
    }
  }, [signalR.connection]);

  return signalR;
};
