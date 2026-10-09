import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";
import { useContext, useEffect, useRef, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/icons";
import { HelperFunctions } from "../../util/helperFunctions";
import { DateFunctions } from "../../util/dateFunctions";
import { UserService } from "../../api/UserService";
import { MessagingService } from "../../api/MessagingService";
import { Container } from "../../components/Layout/Container/Container";
import { AppContext } from "../../context/AppContext";
import { User } from "@typeConfigs/user";
import { Chat, Message } from "@typeConfigs/messaging";

export const Chats = () => {
  const appContext = useContext(AppContext);
  const { user, checkToken } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [unreadUserIds, setUnreadUserIds] = useState<string[]>([]);
  const [openChat, setOpenChat] = useState<Chat | null>(null);
  const [newMessage, setNewMessage] = useState<string>("");
  const [isMessageEmpty, setIsMessageEmpty] = useState<boolean>(false);
  const [messageIsSending, setMessageIsSending] = useState<boolean>(false);
  const messageInputRef = useRef<HTMLInputElement>(null);
  const messageListRef = useRef<HTMLDivElement>(null);

  const [errorMessage, setErrorMessage] = useState("");
  const selectedRecipient = useRef<string | null>(null);
  const selectionVersion = useRef(0);
  const fetchVersion = useRef(0);
  const sending = useRef(false);
  const latestChats = useRef<Chat[] | null>(null);

  const mergeChat = (incoming: Chat, previous?: Chat): Chat => ({
    ...incoming,
    messages: Array.from(
      new Map(
        [...(previous?.messages ?? []), ...incoming.messages].map((message) => [
          message.id,
          message,
        ]),
      ).values(),
    ),
  });

  const getUsers = async () => {
    try {
      setUsers(await UserService.getUsers(user!.token!, user!.id));
    } catch (error) {
      setErrorMessage((error as Error).message);
      checkToken();
    }
  };

  const getChats = async () => {
    const version = ++fetchVersion.current;
    try {
      const response = await MessagingService.getChats(user!.token!);
      if (version !== fetchVersion.current) return;
      const previous = latestChats.current;
      if (previous) {
        const unread: string[] = [];
        response.forEach((chat) => {
          const recipient = chat.users.find((member) => member.id !== user!.id)?.id;
          if (!recipient || recipient === selectedRecipient.current) return;
          const knownIds = new Set(
            previous.find((item) => item.id === chat.id)?.messages.map((message) => message.id),
          );
          chat.messages.forEach((message) => {
            if (message.senderId !== user!.id && !knownIds.has(message.id)) unread.push(recipient);
          });
        });
        setUnreadUserIds((current) => [...current, ...unread]);
      }
      const chats = response.map((chat) =>
        mergeChat(
          chat,
          previous?.find((item) => item.id === chat.id),
        ),
      );
      latestChats.current = chats;
      appContext.dispatch({ type: "LOAD_CHATS", chats });
      setOpenChat((current) => {
        const incoming = chats.find((chat) => chat.id === current?.id);
        return current && incoming ? mergeChat(incoming, current) : null;
      });
    } catch (error) {
      if (version === fetchVersion.current) setErrorMessage((error as Error).message);
      checkToken();
    }
  };

  const startChat = async (recipientId: string, version: number) => {
    try {
      const response = await MessagingService.startChat(recipientId, user!.token!);
      if (version !== selectionVersion.current || selectedRecipient.current !== recipientId) return;
      const chat = mergeChat(
        response,
        latestChats.current?.find((item) => item.id === response.id),
      );
      setOpenChat(chat);
      appContext.dispatch({ type: "STARTED_CHAT", chat });
    } catch (error) {
      if (version === selectionVersion.current) setErrorMessage((error as Error).message);
    }
  };

  const handleUserClick = (recipient: User) => {
    if (selectedRecipient.current === recipient.id && openChat) return;
    selectedRecipient.current = recipient.id!;
    const version = ++selectionVersion.current;
    setSelectedUser(recipient);
    setOpenChat(null);
    setNewMessage("");
    setIsMessageEmpty(false);
    setErrorMessage("");
    setUnreadUserIds((current) => current.filter((id) => id !== recipient.id));
    void startChat(recipient.id!, version);
  };

  const handleMessageSend = async () => {
    if (
      !openChat ||
      !selectedRecipient.current ||
      !openChat.users.some((member) => member.id === selectedRecipient.current) ||
      sending.current
    )
      return;
    if (!newMessage.trim()) {
      setIsMessageEmpty(true);
      return;
    }
    const chatId = openChat.id;
    const version = selectionVersion.current;
    const content = newMessage;
    sending.current = true;
    setMessageIsSending(true);
    setErrorMessage("");
    try {
      const message = await MessagingService.sendMessage({ chatId, content }, user!.token!);
      setOpenChat((current) =>
        current?.id === chatId ? mergeChat({ ...current, messages: [message] }, current) : current,
      );
      latestChats.current =
        latestChats.current?.map((chat) =>
          chat.id === chatId ? mergeChat({ ...chat, messages: [message] }, chat) : chat,
        ) ?? null;
      if (selectionVersion.current === version)
        setNewMessage((current) => (current === content ? "" : current));
      appContext.dispatch({ type: "SENT_MESSAGE", message });
    } catch (error) {
      if (selectionVersion.current === version) setErrorMessage((error as Error).message);
    } finally {
      sending.current = false;
      setMessageIsSending(false);
    }
  };

  const handleKeyDown = async (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      await handleMessageSend();
    }
  };

  const isLastMessageFromRecipientInSeries = (message: Message, messages: Message[]) => {
    if (messages.length === 0) return false;

    const otherUserMessages = messages.filter((m) => m.senderId !== user!.id);

    DateFunctions.sortItemsByDateTimeAttribute(otherUserMessages, "createdAt");

    if (otherUserMessages[otherUserMessages.length - 1]?.id === message.id) return true;
    else return false;
  };

  const getMessagesGroupedByDay = () => {
    return Object.values(HelperFunctions.groupMessagesByDay(openChat!.messages));
  };

  const scrollMessagesToBottom = () => {
    const list = messageListRef.current;

    if (list) {
      list.scrollTop = list.scrollHeight;
    }
  };

  useEffect(() => {
    void getUsers();
    return () => {
      selectionVersion.current++;
      fetchVersion.current++;
    };
  }, [user?.token]);

  useEffect(() => {
    void getChats();
  }, [appContext.lastMessageRegistered, user?.token]);

  useEffect(() => {
    scrollMessagesToBottom();
  }, [openChat?.messages?.length]);

  return (
    <PageLayout className="chat">
      <Container title="Chat" desc="A place to keep the conversation going.">
        {errorMessage && (
          <p role="alert" className="error">
            {errorMessage}
          </p>
        )}
        <div className="chat-container grid min-w-0 gap-4 md:grid-cols-[16rem_minmax(0,1fr)]">
          <div
            className="user-list flex gap-2 overflow-x-auto rounded-2xl border border-line/70 bg-surface p-2 md:max-h-[70dvh] md:flex-col md:overflow-y-auto"
            aria-label="People"
          >
            {users.length === 0 && <p className="p-4 text-sm text-muted">No other users yet.</p>}
            {users?.map((u) => {
              const isActiveChat = openChat?.users?.map((cu) => cu.id)?.includes(u.id);
              const hasUnreadMessages = unreadUserIds?.some((id) => id == u.id);
              const numberOfUnreadMessages = unreadUserIds?.filter((id) => id == u.id).length;
              const unknownName = !u.firstName || !u.lastName;
              const userInitials = unknownName ? "??" : `${u.firstName[0]}${u.lastName[0]}`;
              const userName = unknownName ? u.email : `${u.firstName} ${u.lastName}`;

              return (
                <button
                  type="button"
                  aria-pressed={Boolean(isActiveChat)}
                  className={`user-card flex min-h-16 w-56 shrink-0 items-center gap-3 rounded-xl p-3 text-left transition-colors hover:bg-elevated md:w-full ${isActiveChat ? "active bg-elevated" : ""} ${hasUnreadMessages ? "unread font-semibold" : ""}`}
                  key={u.id}
                  onClick={() => handleUserClick(u)}
                >
                  <span
                    className="user-icon grid size-10 shrink-0 place-items-center rounded-full bg-page text-sm text-mint"
                    aria-hidden="true"
                  >
                    {userInitials}
                  </span>
                  <span className="user-name min-w-0 flex-1 truncate text-sm">{userName}</span>
                  {hasUnreadMessages && (
                    <span
                      className="user-unreads grid size-6 shrink-0 place-items-center rounded-full bg-brand text-xs text-white"
                      aria-label={`${numberOfUnreadMessages} unread messages`}
                    >
                      {numberOfUnreadMessages > 4 ? `+4` : numberOfUnreadMessages}
                    </span>
                  )}
                </button>
              );
            })}
          </div>
          <div className="messages flex h-[65dvh] min-h-80 min-w-0 flex-col overflow-hidden rounded-2xl border border-line/70 bg-surface md:h-[70dvh]">
            {selectedUser && openChat ? (
              <div className="message-container flex min-h-0 flex-1 flex-col">
                <div className="handle flex shrink-0 items-center gap-3 border-b border-line/60 p-4">
                  <span
                    className="user-icon grid size-10 shrink-0 place-items-center rounded-full bg-elevated text-sm text-mint"
                    aria-hidden="true"
                  >{`${selectedUser.firstName?.[0] ?? "?"}${selectedUser.lastName?.[0] ?? "?"}`}</span>
                  <span className="user-name text-sm font-semibold">{`${selectedUser.firstName} ${selectedUser.lastName}`}</span>
                </div>

                <div
                  className="message-list flex min-h-0 flex-1 flex-col gap-3 overflow-y-auto p-4 sm:p-6"
                  ref={messageListRef}
                  role="log"
                  aria-label="Conversation"
                  aria-relevant="additions"
                >
                  {getMessagesGroupedByDay().map((messageList) => {
                    DateFunctions.sortItemsByDateTimeAttribute(messageList, "createdAt");

                    return messageList.map((m, index) => {
                      const isMessageAuthor = m.senderId == user!.id;

                      return (
                        <div
                          className={`message flex w-full flex-col gap-2 ${isMessageAuthor ? "author items-end" : "items-start"}`}
                          key={m.id}
                        >
                          {index === 0 && (
                            <span className="message-date self-center py-2 text-xs text-muted">
                              {HelperFunctions.getMessageTimeLabelAccordingToToday(m.createdAt!)}
                            </span>
                          )}

                          <div className="message-box flex max-w-[90%] items-end gap-2">
                            {!isMessageAuthor && (
                              <span
                                className={`message-handle grid size-7 shrink-0 place-items-center rounded-full bg-elevated text-xs text-mint ${isLastMessageFromRecipientInSeries(m, messageList) ? "show visible" : "invisible"}`}
                                aria-hidden="true"
                              >{`${selectedUser.firstName?.[0] ?? "?"}${selectedUser.lastName?.[0] ?? "?"}`}</span>
                            )}
                            <div
                              className={`message-content min-w-0 rounded-2xl px-4 py-2 text-sm break-words whitespace-pre-wrap ${isMessageAuthor ? "bg-elevated" : "bg-brand/25"}`}
                            >
                              {m.content}
                            </div>
                          </div>
                        </div>
                      );
                    });
                  })}
                </div>

                <div className="message-area flex shrink-0 items-center gap-3 border-t border-line/60 p-4">
                  <input
                    type="text"
                    className={`message-input min-h-11 min-w-0 flex-1 rounded-lg border bg-page px-4 py-2 text-base text-ink placeholder:text-muted focus:outline-none focus:ring-2 focus:ring-mint/30 disabled:opacity-50 ${isMessageEmpty ? "empty border-danger" : "border-line focus:border-mint"}`}
                    placeholder="Write a message…"
                    aria-label="Message"
                    aria-invalid={isMessageEmpty}
                    maxLength={4000}
                    value={newMessage}
                    disabled={messageIsSending}
                    ref={messageInputRef}
                    onChange={(e) => {
                      setNewMessage(e.currentTarget.value);
                      setIsMessageEmpty(false);
                    }}
                    onKeyDown={(e) => handleKeyDown(e)}
                  />
                  <button
                    type="button"
                    aria-label="Send message"
                    disabled={messageIsSending}
                    className="send-icon grid size-11 shrink-0 place-items-center rounded-lg bg-brand text-white transition-colors hover:bg-brand-hover disabled:opacity-50"
                    onClick={() => handleMessageSend()}
                  >
                    {ICONS.SEND_ICON({})}
                  </button>
                </div>
              </div>
            ) : (
              <span
                className="info-message m-auto max-w-sm p-6 text-center text-muted"
                role="status"
              >
                {selectedUser
                  ? "Loading conversation…"
                  : "Start chatting right away by clicking on another user"}
              </span>
            )}
          </div>
        </div>
      </Container>
    </PageLayout>
  );
};
