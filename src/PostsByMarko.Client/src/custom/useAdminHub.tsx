import { useState, useEffect } from "react";
import { HubConnection } from "@microsoft/signalr";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { useAuth } from "./useAuth";
import { createHubConnection } from "./useSignalRConnection";
import { AdminHubEvents } from "../types/signalr";

export const useAdminHub = () => {
  const { user, isAdmin } = useAuth();
  const connection: HubConnection = createHubConnection(
    ENDPOINT_URLS.ADMIN_HUB,
    user?.token!
  );
  const [lastAdminAction, setLastAdminAction] = useState<string>("");

  useEffect(() => {
    if (!isAdmin) {
      throw new Error("User is not an admin!");
    }

    if (connection) {
      connection
        .start()
        .then(() => {
          connection.on(
            AdminHubEvents.UpdatedUserRoles,
            (userId: string, updatedAt: Date) =>
              setLastAdminAction(
                `Updated roles for user with Id '${userId}' at ${updatedAt}`
              )
          );

          connection.on(AdminHubEvents.DeletedUser, (userId: string, deletedAt: Date) =>
            setLastAdminAction(`Deleted user with Id '${userId}' at ${deletedAt}`)
          );
        })
        .catch((error) => console.error(`SignalR connection	failed with error: ${error}`));
    }

    return () => {
      connection.stop();
    };
  }, [connection]);

  return lastAdminAction;
};
