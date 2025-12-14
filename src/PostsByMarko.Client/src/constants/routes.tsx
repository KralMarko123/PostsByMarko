export const POST_DETAILS_PREFIX = "/posts";
export const ROUTES: Record<string, string> = {
  HOME: "/",
  DETAILS: `${POST_DETAILS_PREFIX}/:id`,
  CHAT: "/chat",
  LOGIN: `/login`,
  REGISTER: `/register`,
  ADMIN: "/admin",
  TEST: `/test`,
  NOT_FOUND: "*",
};
