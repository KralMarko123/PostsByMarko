import { User } from "./user";

export interface Post {
  id?: string | null;
  authorId?: string | null;
  author?: User | null;
  title: string;
  content: string;
  createdAt?: Date | null;
  lastUpdatedAt?: Date | null;
  hidden: boolean;
}

export interface CreatePostRequest {
  title: string;
  content: string;
}

export interface UpdatePostRequest {
  title: string;
  content: string;
  hidden: boolean;
}
