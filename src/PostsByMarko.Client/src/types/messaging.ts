import { User } from "./user";

export interface Message {
  id?: string | null;
  chatId?: string | null;
  senderId?: string | null;
  content: string;
  createdAt?: string | null;
}

export interface Chat {
  id: string;
  createdAt: string | null;
  updatedAt: string;
  users: User[];
  messages: Message[];
}
