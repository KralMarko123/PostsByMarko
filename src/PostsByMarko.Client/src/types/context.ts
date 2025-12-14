import { ActionDispatch } from "react";
import { Chat, Message } from "./messaging";
import { Post } from "./post";

export interface AppProviderProps {
  children: React.ReactNode;
}

export interface AppContextValue {
  posts: Post[];
  modalVisibility: Record<string, boolean>;
  postBeingModified: Post & {
    index?: number | null;
  };
  chats: Chat[];
  dispatch: ActionDispatch<[action: AppAction]>;
}

export type AppAction =
  // MODAL EVENTS
  | { type: "SHOW_MODAL"; modal: string }
  | { type: "CLOSE_MODAL"; modal: string }

  // POST EVENTS
  | { type: "LOAD_POSTS"; posts: Post[] }
  | { type: "DELETED_POST"; id: string }
  | { type: "UPDATED_POST"; post: Post }
  | { type: "CREATED_POST"; post: Post }
  | {
      type: "MODIFYING_POST";
      post: Post & {
        index?: number | null;
      };
    }

  // CHAT EVENTS
  | { type: "LOAD_CHATS"; chats: Chat[] }
  | { type: "SENT_MESSAGE"; message: Message }
  | { type: "STARTED_CHAT"; chat: Chat };
