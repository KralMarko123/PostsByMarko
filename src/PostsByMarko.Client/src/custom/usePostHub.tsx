import { Dispatch } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { AppAction } from "../types/context";
import { useSignalRHub } from "./useSignalRConnection";

const events = ["PostCreated", "PostUpdated", "PostDeleted"] as const;

export const usePostHub = (token: string | null | undefined, dispatch: Dispatch<AppAction>) =>
  useSignalRHub(ENDPOINT_URLS.POST_HUB, token, dispatch, events, "MESSAGE_REGISTERED");
