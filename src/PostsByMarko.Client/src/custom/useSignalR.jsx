import { React, useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT__URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const useSignalR = (isForPostHub = true) => {
  const hubUrl = isForPostHub
    ? ENDPOINT__URLS.POST_HUB
    : ENDPOINT__URLS.MESSAGE_HUB;
  const { user } = useAuth();
  const [signalR, setSignalR] = useState({
    connection: new HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => user.token,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build(),

    async sendMessage(payload, toAll = true) {
      if (isForPostHub) {
        await sendMessageToPostHub(payload, toAll);
      } else notifyUserIdsForNewMessage(payload);
    },

    lastMessageRegistered: "",
  });

  useEffect(() => {
    if (signalR.connection) {
      signalR.connection
        .start()
        .then(() => {
          signalR.connection.on("ReceiveMessage", (message) => {
            setSignalR({ ...signalR, lastMessageRegistered: message });
          });
        })
        .catch((error) =>
          console.error(`SignalR connection	failed with error: ${error}`)
        );
    }
  }, [signalR.connection]);

  const sendMessageToPostHub = async (message, toAll = false) => {
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

  const notifyUserIdsForNewMessage = async (userIds) => {
    if (signalR.connection) {
      try {
        await signalR.connection.invoke("NotifyUsersAboutNewMessage", userIds);
      } catch (error) {
        console.error(error);
      }
    } else
      console.log("A connection to the server hasn't been established yet!");
  };

  return signalR;
};
