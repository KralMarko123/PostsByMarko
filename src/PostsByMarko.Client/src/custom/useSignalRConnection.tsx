import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export const createHubConnection = (url: string, token: string): HubConnection => {
  return new HubConnectionBuilder()
    .withUrl(url, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect([0, 2, 4, 6, 8, 10, 15, 20, 30])
    .configureLogging(LogLevel.Information)
    .build();
};
