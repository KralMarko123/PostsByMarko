export enum PostHubEvents {
  PostCreated = "PostCreated",
  PostUpdated = "PostUpdated",
  PostDeleted = "PostDeleted",
}

export enum AdminHubEvents {
  UpdatedUserRoles = "UpdatedUserRoles",
  DeletedUser = "DeletedUser",
}

export enum MessageHubEvents {
  MessageSent = "MessageSent",
  ChatCreated = "ChatCreated",
}
