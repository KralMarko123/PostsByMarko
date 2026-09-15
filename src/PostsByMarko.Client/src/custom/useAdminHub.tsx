import { Dispatch } from "react";
import { ENDPOINT_URLS } from "../constants/endpoints";
import { AppAction } from "../types/context";
import { useSignalRHub } from "./useSignalRConnection";

const events = ["UpdatedUserRoles", "DeletedUser"] as const;

export const useAdminHub = (token: string | null | undefined, dispatch: Dispatch<AppAction>) =>
  useSignalRHub(ENDPOINT_URLS.ADMIN_HUB, token, dispatch, events, "ADMIN_ACTION_REGISTERED");
