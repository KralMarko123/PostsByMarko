export interface Endpoints {
  // Admin
  GET_ROLES_FOR_EMAIL: string;
  GET_DASHBOARD: string;
  DELETE_USER: (id: string) => string;
  UPDATE_USER_ROLES: string;

  // Auth
  LOGIN: string;
  REGISTER: string;
  VALIDATE: string;

  // User
  GET_USERS: string;
  GET_USER: (id: string) => string;

  // Posts
  GET_POSTS: string;
  GET_POST_BY_ID: (id: string) => string;
  CREATE_POST: string;
  UPDATE_POST: (id: string) => string;
  DELETE_POST: (id: string) => string;

  // Messaging
  GET_CHATS: string;
  START_CHAT: (id: string) => string;
  SEND_MESSAGE: string;

  // Hubs
  POST_HUB: string;
  MESSAGE_HUB: string;
  ADMIN_HUB: string;
}
