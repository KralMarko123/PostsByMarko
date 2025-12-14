import { AppAction, AppContextValue } from "@typeConfigs/context";
import { Chat } from "@typeConfigs/messaging";
import { Post } from "@typeConfigs/post";

export const AppReducer = (state: AppContextValue, action: AppAction) => {
  let posts: Post[];
  let postBeingModifiedIndex: number;
  let chats: Chat[];

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
      return { ...state, postBeingModified: { ...action.post } };

    case "DELETED_POST":
      return {
        ...state,
        posts: [...state.posts.filter((p) => p.id !== action.id)],
      };

    case "UPDATED_POST":
      postBeingModifiedIndex = [...state.posts].findIndex((p) => p.id === action.post.id);
      posts = [...state.posts];
      posts[postBeingModifiedIndex] = { ...action.post };

      return { ...state, posts: posts };

    case "CREATED_POST":
      posts = [...state.posts];

      posts.push(action.post);

      return { ...state, posts: posts };

    // CHAT EVENTS
    case "LOAD_CHATS":
      return { ...state, chats: action.chats };

    case "SENT_MESSAGE":
      let newMessage = action.message;

      chats = [...state.chats];

      let indexOfChatForMessage = chats.findIndex((c) => c.id === newMessage.chatId);

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

    // SIGNALR EVENTS
    case "MESSAGE_REGISTERED":
      return { ...state, lastMessageRegistered: action.message };

    case "ADMIN_ACTION_REGISTERED":
      return { ...state, lastAdminAction: action.message };

    default:
      return state;
  }
};
