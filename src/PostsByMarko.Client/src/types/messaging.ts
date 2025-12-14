import { User } from "./user";

export interface Message {
  id?: string | null;
  chatId?: string | null;
  senderId?: string | null;
  content: string;
  createdAt: Date;
}

export interface Chat {
  id: string;
  createdAt: Date;
  updatedAt: Date;
  users: User[];
  messages: Message[];
}
