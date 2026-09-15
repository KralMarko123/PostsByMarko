import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { Dispatch, useEffect } from "react";
import { AppAction } from "../types/context";

export const createHubConnection = (url: string, token: string): HubConnection =>
  new HubConnectionBuilder()
    .withUrl(url, { accessTokenFactory: () => token })
    .withAutomaticReconnect([0, 2000, 10000, 30000])
    .configureLogging(LogLevel.Warning)
    .build();

export const startHubConnection = (connection: HubConnection, onReady: () => void) => {
  let disposed = false;
  let retryTimer: ReturnType<typeof setTimeout> | undefined;
  let attempts = 0;
  const scheduleStart = () => {
    if (disposed || retryTimer !== undefined) return;
    retryTimer = setTimeout(() => {
      retryTimer = undefined;
      void start();
    }, Math.min(1000 * 2 ** Math.min(attempts++, 5), 30000));
  };
  const start = async () => {
    try {
      await connection.start();
      if (disposed) { await connection.stop(); return; }
      attempts = 0;
      onReady();
    } catch {
      scheduleStart();
    }
  };
  connection.onreconnected(() => { if (!disposed) onReady(); });
  connection.onclose(scheduleStart);
  void start();
  return () => {
    disposed = true;
    clearTimeout(retryTimer);
    void connection.stop().catch(() => undefined);
  };
};

export const useSignalRHub = (
  url: string,
  token: string | null | undefined,
  dispatch: Dispatch<AppAction>,
  events: readonly string[],
  actionType: "MESSAGE_REGISTERED" | "ADMIN_ACTION_REGISTERED"
) => {
  useEffect(() => {
    if (!token) return;
    const connection = createHubConnection(url, token);
    let revision = 0;
    const notify = () => dispatch({ type: actionType, message: `${url}:${++revision}:${Date.now()}` });
    // Subscribe before connecting, then refresh after connection/reconnection to cover missed events.
    events.forEach(event => connection.on(event, notify));
    const stop = startHubConnection(connection, notify);
    return () => {
      events.forEach(event => connection.off(event, notify));
      stop();
    };
  }, [url, token, dispatch, events, actionType]);
};
