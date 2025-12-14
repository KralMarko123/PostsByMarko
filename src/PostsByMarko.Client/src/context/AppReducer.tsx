import { AppAction, AppContextValue } from "@typeConfigs/context";

export const AppReducer = (state: AppContextValue, action: AppAction) => {
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
      return {
        ...state,
        posts: state.posts.map((p) =>
          p.id === action.post.id ? { ...action.post } : { ...p }
        ),
      };

    case "CREATED_POST":
      return { ...state, posts: [...state.posts, action.post] };

    // CHAT EVENTS
    case "LOAD_CHATS":
      return { ...state, chats: action.chats };

    case "SENT_MESSAGE":
      let newMessage = action.message;

      return {
        ...state,
        chats: state.chats.map((c) =>
          c.id === newMessage.chatId ? { ...c, messages: [...c.messages, newMessage] } : c
        ),
      };

    case "STARTED_CHAT":
      const newChat = action.chat;
      const alreadyExists = state.chats.some((c) => c.id === newChat.id);

      return {
        ...state,
        chats: alreadyExists
          ? state.chats.map((c) => (c.id === newChat.id ? newChat : c))
          : [...state.chats, newChat],
      };

    // SIGNALR EVENTS
    case "MESSAGE_REGISTERED":
      return { ...state, lastMessageRegistered: action.message };

    case "ADMIN_ACTION_REGISTERED":
      return { ...state, lastAdminAction: action.message };

    default:
      return state;
  }
};
