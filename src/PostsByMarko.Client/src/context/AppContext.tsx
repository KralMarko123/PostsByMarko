import { createContext } from "react";
import { AppContextValue } from "types/context";

export const AppContext = createContext<AppContextValue>({
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
});
