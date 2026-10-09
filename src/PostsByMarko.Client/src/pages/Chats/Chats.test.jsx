import React, { act } from "react";
import { createRoot } from "react-dom/client";
import { Chats } from "./Chats";
import { MessagingService } from "../../api/MessagingService";
import { UserService } from "../../api/UserService";
import { vi } from "vitest";

vi.mock("../../custom/useAuth", () => ({
  useAuth: () => ({ user: { id: "me", token: "token" }, checkToken: vi.fn() }),
}));
vi.mock("../../api/MessagingService");
vi.mock("../../api/UserService");
vi.mock("../../components/Layout/Nav/Nav", () => ({ Nav: () => null }));
vi.mock("../../components/Layout/Logo/Logo", () => ({ Logo: () => null }));
vi.mock("../../components/Layout/Footer/Footer", () => ({ Footer: () => null }));
global.IS_REACT_ACT_ENVIRONMENT = true;

const alice = { id: "alice", firstName: "Alice", lastName: "A" };
const bob = { id: "bob", firstName: "Bob", lastName: "B" };
const chat = (recipient) => ({ id: recipient.id, users: [{ id: "me" }, recipient], messages: [] });
let container, root;
beforeEach(async () => {
  vi.clearAllMocks();
  UserService.getUsers.mockResolvedValue([alice, bob]);
  MessagingService.getChats.mockResolvedValue([]);
  container = document.createElement("div");
  root = createRoot(container);
  await act(async () => root.render(<Chats />));
});
afterEach(async () => {
  await act(async () => root.unmount());
});

test("switching recipient removes the old composer and ignores late selection responses", async () => {
  // Arrange
  let finishAlice, finishBob;
  MessagingService.startChat.mockImplementation(
    (id) =>
      new Promise((resolve) => {
        if (id === "alice") finishAlice = resolve;
        else finishBob = resolve;
      }),
  );
  const cards = container.querySelectorAll(".user-card");
  // Act
  await act(async () => cards[0].click());
  await act(async () => cards[1].click());
  // Assert
  expect(container.querySelector(".message-input")).toBeNull();

  // Act
  await act(async () => finishBob(chat(bob)));
  // Assert
  expect(container.querySelector(".handle").textContent).toContain("Bob B");

  // Act
  await act(async () => finishAlice(chat(alice)));
  // Assert
  expect(container.querySelector(".handle").textContent).toContain("Bob B");
});

test("an established conversation is hidden immediately when another recipient is selected", async () => {
  // Arrange
  MessagingService.startChat
    .mockResolvedValueOnce(chat(alice))
    .mockImplementation(() => new Promise(() => {}));
  const cards = container.querySelectorAll(".user-card");
  // Act
  await act(async () => cards[0].click());

  // Assert
  expect(container.querySelector(".message-input")).not.toBeNull();
  // Act
  await act(async () => cards[1].click());

  // Assert
  expect(container.querySelector(".message-input")).toBeNull();
  expect(MessagingService.sendMessage).not.toHaveBeenCalled();
});
