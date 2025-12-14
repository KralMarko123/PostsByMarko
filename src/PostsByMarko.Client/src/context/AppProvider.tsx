import { useReducer } from "react";
import { AppContext } from "./AppContext";
import { AppContextValue, AppProviderProps } from "../types/context";
import { AppReducer } from "./AppReducer";
import { usePostHub } from "../custom/usePostHub";
import { useAuth } from "../custom/useAuth";
import { useMessageHub } from "../custom/useMessageHub";
import { useAdminHub } from "../custom/useAdminHub";

export const defaultAppState: AppContextValue = {
  posts: [],
  modalVisibility: {
    createPost: false,
    updatePost: false,
    deletePost: false,
    addUserToPost: false,
  },
  postBeingModified: {
    id: null,
    authorId: null,
    author: null,
    title: "",
    content: "",
    createdAt: null,
    lastUpdatedAt: null,
    hidden: false,
    index: null,
  },
  chats: [],

  lastMessageRegistered: "",
  lastAdminAction: "",

  dispatch: () => null,
};

export const AppProvider = (props: AppProviderProps) => {
  const { user } = useAuth();
  const [appState, dispatch] = useReducer(AppReducer, defaultAppState);

  const appContext: AppContextValue = {
    posts: appState.posts,
    modalVisibility: appState.modalVisibility,
    postBeingModified: appState.postBeingModified,
    chats: appState.chats,

    lastMessageRegistered: appState.lastMessageRegistered,
    lastAdminAction: appState.lastAdminAction,

    dispatch: dispatch,
  };

  usePostHub(user?.token, dispatch);
  useMessageHub(user?.token, dispatch);
  useAdminHub(user?.token, dispatch);

  return <AppContext.Provider value={appContext}>{props.children}</AppContext.Provider>;
};
