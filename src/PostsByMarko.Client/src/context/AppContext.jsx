import { React, createContext } from "react";

const AppContext = createContext({
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

export default AppContext;
