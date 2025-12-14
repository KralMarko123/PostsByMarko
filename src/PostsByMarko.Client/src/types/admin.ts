import { ActionType } from "constants/enums";

export interface AdminDashboardResponse {
  userId: string;
  email: string;
  numberOfPosts: number;
  lastPostedAt: string;
  roles: string[];
}

export interface UpdateUserRolesRequest {
  userId: string;
  actionType: ActionType;
  role: string;
}
