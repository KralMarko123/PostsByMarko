import { React, useReducer } from "react";
import AppContext from "./AppContext";

export const defaultAppState = {
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
  chats: [],
};

export const appReducer = (state, action) => {
  let posts;
  let postBeingModifiedIndex;
  let chats;

  switch (action.type) {
    // MODAL EVENTS

    case "SHOW_MODAL":
      return {
        ...state,
        modalVisibility: { ...state.modalVisibility, [action.modal]: true },
      };

    case "CLOSE_MODAL":
      return {
        ...state,
        modalVisibility: { ...state.modalVisibility, [action.modal]: false },
      };

    // POST EVENTS

    case "LOAD_POSTS":
      return { ...state, posts: action.posts };

    case "MODIFYING_POST":
      return { ...state, postBeingModified: action.post };

    case "DELETED_POST":
      return {
        ...state,
        posts: [...state.posts.filter((p) => p.id !== action.id)],
      };

    case "UPDATED_POST":
      postBeingModifiedIndex = [...state.posts].findIndex(
        (p) => p.id === action.post.id
      );
      posts = [...state.posts];

      posts[postBeingModifiedIndex].title = action.post.title;
      posts[postBeingModifiedIndex].content = action.post.content;
      posts[postBeingModifiedIndex].lastUpdatedDate = new Date().toISOString();

      return { ...state, posts: posts };

    case "CREATED_POST":
      posts = [...state.posts];

      posts.push(action.post);

      return { ...state, posts: posts };

    case "TOGGLE_POST_HIDDEN":
      postBeingModifiedIndex = [...state.posts].findIndex(
        (p) => p.id === action.id
      );
      posts = [...state.posts];

      posts[postBeingModifiedIndex].isHidden =
        !posts[postBeingModifiedIndex].isHidden;
      posts[postBeingModifiedIndex].lastUpdatedDate = new Date().toISOString();

      return { ...state, posts: posts };

    case "TOGGLED_USER":
      postBeingModifiedIndex = [...state.posts].findIndex(
        (p) => p.id == action.id
      );
      posts = [...state.posts];

      if (action.isAdded)
        posts[postBeingModifiedIndex].allowedUsers.push(action.username);
      else
        posts[postBeingModifiedIndex].allowedUsers = posts[
          postBeingModifiedIndex
        ].allowedUsers.filter((u) => u !== action.username);

      return { ...state, posts: posts };

    // CHAT EVENTS
    case "LOAD_CHATS":
      return { ...state, chats: action.chats };

    case "SENT_MESSAGE":
      let newMessage = action.message;

      chats = [...state.chats];

      let indexOfChatForMessage = chats.findIndex(
        (c) => c.id === newMessage.chatId
      );

      if (indexOfChatForMessage !== -1) {
        chats[indexOfChatForMessage].messages.push(newMessage);
      }

      return { ...state, chats: chats };

    case "STARTED_CHAT":
      chats = [...state.chats];
      let updatedChat = action.chat;
      let indexOfExistingChat = chats.findIndex((c) => c.id === updatedChat.id);

      if (indexOfExistingChat === -1) {
        chats.push(updatedChat);
      } else {
        chats[indexOfExistingChat] = updatedChat;
      }

      return { ...state, chats: chats };

    default:
      return defaultAppState;
  }
};

const AppProvider = ({ children }) => {
  const [appState, dispatch] = useReducer(appReducer, defaultAppState);

  const appContext = {
    posts: appState.posts,
    modalVisibility: appState.modalVisibility,
    postBeingModified: appState.postBeingModified,
    chats: appState.chats,

    dispatch: dispatch,
  };

  return (
    <AppContext.Provider value={appContext}>{children}</AppContext.Provider>
  );
};

export default AppProvider;
