import { AppReducer } from "./AppReducer";

test("a mutation response does not duplicate an entity already received by refetch", () => {
  // Arrange
  const post = { id: "post", title: "Saved" };
  const message = { id: "message", chatId: "chat", content: "Saved" };
  const state = { posts: [post], chats: [{ id: "chat", messages: [message] }] };
  // Act
  const afterPost = AppReducer(state, { type: "CREATED_POST", post });
  const afterMessage = AppReducer(afterPost, { type: "SENT_MESSAGE", message });
  // Assert
  expect(afterMessage.posts).toEqual([post]);
  expect(afterMessage.chats[0].messages).toEqual([message]);
});
