import { React, useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT__URLS from "../constants/endpoints";
import { useAuth } from "./useAuth";

export const useSignalR = () => {
	const { user } = useAuth();
	const [signalR, setSignalR] = useState({
		connection: new HubConnectionBuilder()
			.withUrl(ENDPOINT__URLS.HUB, {
				accessTokenFactory: () => user.token,
			})
			.withAutomaticReconnect()
			.configureLogging(LogLevel.Information)
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
				.catch((error) => console.error(`Connection failed with error: ${error}`));
		}
	}, [signalR.connection]);

	const sendMessageToHub = async (message, toAll) => {
		if (signalR.connection) {
			try {
				await signalR.connection.send(toAll ? "SendMessageToAll" : "SendMessageToOthers", message);
			} catch (error) {
				console.error(error);
			}
		} else console.log("A connection to the server hasn't been established yet!");
	};

	return signalR;
};
