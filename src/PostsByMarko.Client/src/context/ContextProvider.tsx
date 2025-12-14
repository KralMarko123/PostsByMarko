import { useReducer } from "react";
import { AppContext } from "./AppContext";
import { AppContextValue, AppProviderProps } from "../types/context";
import { AppReducer } from "./AppReducer";

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
  dispatch: () => null,
};

export const AppProvider = (props: AppProviderProps) => {
  const [appState, dispatch] = useReducer(AppReducer, defaultAppState);

  const appContext: AppContextValue = {
    posts: appState.posts,
    modalVisibility: appState.modalVisibility,
    postBeingModified: appState.postBeingModified,
    chats: appState.chats,

    dispatch: dispatch,
  };

  return <AppContext.Provider value={appContext}>{props.children}</AppContext.Provider>;
};
