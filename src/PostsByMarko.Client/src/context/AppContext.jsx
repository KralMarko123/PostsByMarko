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
    title: "",
    content: "",
    index: null,
  },
});

export default AppContext;
