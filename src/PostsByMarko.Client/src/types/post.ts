import { User } from "./user";

export interface Post {
  id?: string | null;
  authorId?: string | null;
  author?: User | null;
  title: string;
  content: string;
  createdAt?: string | null;
  lastUpdatedAt?: string | null;
  hidden: boolean;
}

export interface PostProps extends Post {
  index: number;
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
