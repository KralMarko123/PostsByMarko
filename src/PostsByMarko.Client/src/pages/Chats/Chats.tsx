import { useContext, useEffect, useRef, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/icons";
import { HelperFunctions } from "../../util/helperFunctions";
import { DateFunctions } from "../../util/dateFunctions";
import { UserService } from "../../api/UserService";
import { MessagingService } from "../../api/MessagingService";
import { Nav } from "../../components/Layout/Nav/Nav";
import { Container } from "../../components/Layout/Container/Container";
import { Footer } from "../../components/Layout/Footer/Footer";
import { Logo } from "../../components/Layout/Logo/Logo";
import { AppContext } from "../../context/AppContext";
import { User } from "@typeConfigs/user";
import { Chat, Message } from "@typeConfigs/messaging";
import "../Page.css";
import "./Chats.css";

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
    messages: Array.from(new Map([
      ...(previous?.messages ?? []), ...incoming.messages,
    ].map(message => [message.id, message])).values()),
  });

  const getUsers = async () => {
    try { setUsers(await UserService.getUsers(user!.token!, user!.id)); }
    catch (error) { setErrorMessage((error as Error).message); checkToken(); }
  };

  const getChats = async () => {
    const version = ++fetchVersion.current;
    try {
      const response = await MessagingService.getChats(user!.token!);
      if (version !== fetchVersion.current) return;
      const previous = latestChats.current;
      if (previous) {
        const unread: string[] = [];
        response.forEach(chat => {
          const recipient = chat.users.find(member => member.id !== user!.id)?.id;
          if (!recipient || recipient === selectedRecipient.current) return;
          const knownIds = new Set(previous.find(item => item.id === chat.id)?.messages.map(message => message.id));
          chat.messages.forEach(message => {
            if (message.senderId !== user!.id && !knownIds.has(message.id)) unread.push(recipient);
          });
        });
        setUnreadUserIds(current => [...current, ...unread]);
      }
      const chats = response.map(chat => mergeChat(chat, previous?.find(item => item.id === chat.id)));
      latestChats.current = chats;
      appContext.dispatch({ type: "LOAD_CHATS", chats });
      setOpenChat(current => {
        const incoming = chats.find(chat => chat.id === current?.id);
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
      const chat = mergeChat(response, latestChats.current?.find(item => item.id === response.id));
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
    setUnreadUserIds(current => current.filter(id => id !== recipient.id));
    void startChat(recipient.id!, version);
  };

  const handleMessageSend = async () => {
    if (!openChat || !selectedRecipient.current ||
        !openChat.users.some(member => member.id === selectedRecipient.current) || sending.current) return;
    if (!newMessage.trim()) { setIsMessageEmpty(true); return; }
    const chatId = openChat.id;
    const version = selectionVersion.current;
    const content = newMessage;
    sending.current = true;
    setMessageIsSending(true);
    setErrorMessage("");
    try {
      const message = await MessagingService.sendMessage({ chatId, content }, user!.token!);
      setOpenChat(current => current?.id === chatId
        ? mergeChat({ ...current, messages: [message] }, current) : current);
      latestChats.current = latestChats.current?.map(chat => chat.id === chatId
        ? mergeChat({ ...chat, messages: [message] }, chat) : chat) ?? null;
      if (selectionVersion.current === version) setNewMessage(current => current === content ? "" : current);
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

    let otherUserMessages = messages.filter((m) => m.senderId !== user!.id);

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
    return () => { selectionVersion.current++; fetchVersion.current++; };
  }, [user?.token]);

  useEffect(() => {
    void getChats();
  }, [appContext.lastMessageRegistered, user?.token]);

  useEffect(() => {
    scrollMessagesToBottom();
  }, [openChat?.messages?.length]);

  return (
    <div className="page chat">
      <Logo />
      <Nav />

      <Container>
        {errorMessage && <p role="alert" className="error">{errorMessage}</p>}
        <div className="chat-container">
          <div className="user-list">
            {users?.map((u) => {
              let isActiveChat = openChat?.users?.map((cu) => cu.id)?.includes(u.id);
              let hasUnreadMessages = unreadUserIds?.some((id) => id == u.id);
              let numberOfUnreadMessages = unreadUserIds?.filter(
                (id) => id == u.id
              ).length;
              let unknownName = !u.firstName || !u.lastName;
              let userInitials = unknownName ? "??" : `${u.firstName[0]}${u.lastName[0]}`;
              let userName = unknownName ? u.email : `${u.firstName} ${u.lastName}`;

              return (
                <div
                  className={`user-card${isActiveChat ? " active" : ""}${
                    hasUnreadMessages ? " unread" : ""
                  }`}
                  key={u.id}
                  onClick={() => handleUserClick(u)}
                >
                  <span className="user-icon">{userInitials}</span>
                  <span className="user-name">{userName}</span>
                  {hasUnreadMessages && (
                    <span className="user-unreads">
                      {numberOfUnreadMessages > 4 ? `+4` : numberOfUnreadMessages}
                    </span>
                  )}
                </div>
              );
            })}
          </div>
          <div className="messages">
            {selectedUser && openChat ? (
              <div className="message-container">
                <div className="handle">
                  <span className="user-icon">{`${selectedUser.firstName?.[0] ?? "?"}${selectedUser.lastName?.[0] ?? "?"}`}</span>
                  <span className="user-name">{`${selectedUser.firstName} ${selectedUser.lastName}`}</span>
                </div>

                <div className="message-list" ref={messageListRef}>
                  {getMessagesGroupedByDay().map((messageList) => {
                    DateFunctions.sortItemsByDateTimeAttribute(messageList, "createdAt");

                    return messageList.map((m, index) => {
                      let isMessageAuthor = m.senderId == user!.id;

                      return (
                        <div
                          className={`message${isMessageAuthor ? " author" : ""}`}
                          key={m.id}
                        >
                          {index === 0 && (
                            <span className="message-date">
                              {HelperFunctions.getMessageTimeLabelAccordingToToday(
                                m.createdAt!
                              )}
                            </span>
                          )}

                          <div className="message-box">
                            {!isMessageAuthor && (
                              <span
                                className={`message-handle${
                                  isLastMessageFromRecipientInSeries(m, messageList)
                                    ? " show"
                                    : ""
                                }`}
                              >{`${selectedUser.firstName?.[0] ?? "?"}${selectedUser.lastName?.[0] ?? "?"}`}</span>
                            )}
                            <div className="message-content">{m.content}</div>
                          </div>
                        </div>
                      );
                    });
                  })}
                </div>

                <div className="message-area">
                  <input
                    type="text"
                    className={`message-input${isMessageEmpty ? " empty" : ""}`}
                    placeholder="Aa"
                    aria-label="Message"
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
                  <button type="button" aria-label="Send message" disabled={messageIsSending} className="send-icon" onClick={() => handleMessageSend()}>
                    {ICONS.SEND_ICON({})}
                  </button>
                </div>
              </div>
            ) : (
              <span className="info-message">
                {selectedUser ? "Loading conversation…" : "Start chatting right away by clicking on another user"}
              </span>
            )}
          </div>
        </div>
      </Container>

      <Footer />
    </div>
  );
};
