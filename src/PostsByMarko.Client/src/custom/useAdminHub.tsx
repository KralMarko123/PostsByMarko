import { Dispatch, useEffect, useRef } from "react";
import { HubConnection } from "@microsoft/signalr";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { createHubConnection } from "./useSignalRConnection";
import { AdminHubEvents } from "../types/signalr";
import { AppAction } from "@typeConfigs/context";

export const useAdminHub = (
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

    const connection: HubConnection = createHubConnection(ENDPOINT_URLS.ADMIN_HUB, token);

    connectionRef.current = connection;

    if (connection) {
      connection
        .start()
        .then(() => {
          connection.on(
            AdminHubEvents.UpdatedUserRoles,
            (userId: string, updatedAt: Date) =>
              dispatch({
                type: "ADMIN_ACTION_REGISTERED",
                message: `Updated roles for user with Id '${userId}' at ${updatedAt}`,
              })
          );

          connection.on(AdminHubEvents.DeletedUser, (userId: string, deletedAt: Date) =>
            dispatch({
              type: "ADMIN_ACTION_REGISTERED",
              message: `Deleted user with Id '${userId}' at ${deletedAt}`,
            })
          );
        })
        .catch((error) => console.error(`SignalR connection	failed with error: ${error}`));
    }
  }, [token, dispatch]);
};
