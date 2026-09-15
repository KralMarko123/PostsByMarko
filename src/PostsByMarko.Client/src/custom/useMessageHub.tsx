import { Dispatch } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { AppAction } from "../types/context";
import { useSignalRHub } from "./useSignalRConnection";

const events = ["MessageSent", "ChatCreated"] as const;

export const useMessageHub = (token: string | null | undefined, dispatch: Dispatch<AppAction>) =>
  useSignalRHub(ENDPOINT_URLS.MESSAGE_HUB, token, dispatch, events, "MESSAGE_REGISTERED");
