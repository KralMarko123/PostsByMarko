import { React, useState, useEffect } from "react";
import {
  HttpTransportType,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import ENDPOINT__URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const useSignalR = () => {
  const { user } = useAuth();
  const [signalR, setSignalR] = useState({
    connection: new HubConnectionBuilder()
      .withUrl(ENDPOINT__URLS.HUB, {
        accessTokenFactory: () => user.token,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build(),

    async sendMessage(message, toAll = true) {
      await sendMessageToHub(message, toAll);
    },

    lastMessageRegistered: "",
  });

  useEffect(() => {
    if (signalR.connection) {
      signalR.connection
        .start()
        .then(() => {
          signalR.connection.on("ReceiveMessage", (message) => {
            console.log(message);
            setSignalR({ ...signalR, lastMessageRegistered: message });
          });
        })
        .catch((error) =>
          console.error(`SignalR connection	failed with error: ${error}`)
        );
    }
  }, [signalR.connection]);

  const sendMessageToHub = async (message, toAll = false) => {
    if (signalR.connection) {
      try {
        await signalR.connection.invoke(
          toAll ? "SendMessageToAll" : "SendMessageToOthers",
          message
        );
      } catch (error) {
        console.error(error);
      }
    } else
      console.log("A connection to the server hasn't been established yet!");
  };

  return signalR;
};
