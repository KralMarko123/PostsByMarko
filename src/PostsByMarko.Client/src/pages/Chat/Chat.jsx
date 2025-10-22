import React, { useContext, useEffect, useMemo, useRef, useState } from "react";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import UsersService from "../../api/UsersService";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import MessagingService from "../../api/MessagingService";
import { ICONS } from "../../constants/icons";
import { HelperFunctions } from "../../util/helperFunctions";
import AppContext from "../../context/AppContext";
import "../Page.css";
import "./Chat.css";

const Chat = () => {
  const appContext = useContext(AppContext);
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState(null);
  const [unreadChats, setUnreadChats] = useState([]);
  const [openChat, setOpenChat] = useState(null);
  const { user } = useAuth();
  const { sendMessage, lastMessageRegistered } = useSignalR(false);
  const [newMessage, setNewMessage] = useState("");
  const [isMessageEmpty, setIsMessageEmpty] = useState(false);
  const messageInputRef = useRef(null);
  const messageListRef = useRef(null);
  const [messageIsSending, setMessageIsSending] = useState(false);

  const getUsers = async () => {
    await UsersService.getOtherUsers(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setUsers(requestResult.payload);
      }
    });
  };

  const getChats = async () => {
    await MessagingService.getChats(user.token).then((requestResult) => {
      appContext.chats.forEach((chat) => {
        let existingChat = requestResult.payload.find(
          (c) => c.id === chat.id && c.id !== openChat?.id
        );

        if (existingChat && existingChat.updatedAt > chat.updatedAt) {
          let userIdWithNewMessage = existingChat.participantIds.filter(
            (id) => id !== user.id
          )[0];

          setUnreadChats([...unreadChats, userIdWithNewMessage]);
        }
      });

      appContext.dispatch({ type: "LOAD_CHATS", chats: requestResult.payload });
    });
  };

  const getChat = async (recipientId) => {
    let participantIds = [user.id, recipientId];

    await MessagingService.getChatByParticipantIds(
      participantIds,
      user.token
    ).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setOpenChat(requestResult.payload);

        appContext.dispatch({
          type: "STARTED_CHAT",
          chat: requestResult.payload,
        });
      }
    });
  };

  const handleUserClick = (u) => {
    if (newMessage.length > 0 && selectedUser !== u) {
      setNewMessage("");
      messageInputRef.current.value = "";
    }

    setSelectedUser(u);
    getChat(u.id);
    setUnreadChats(unreadChats.filter((id) => id !== u.id));
  };

  const handleMessageSend = async () => {
    if (newMessage.trim().length === 0) {
      setIsMessageEmpty(true);
      return;
    } else if (messageIsSending) {
      return;
    }

    setMessageIsSending(true);

    const messageToSend = {
      chatId: openChat.id,
      senderId: user.id,
      content: newMessage,
    };

    await MessagingService.sendMessage(messageToSend, user.token)
      .then((requestResult) => {
        if (requestResult.statusCode === 200) {
          const newMessage = requestResult.payload;

          messageInputRef.current.value = "";
          setNewMessage("");

          setOpenChat({
            ...openChat,
            messages: [...openChat.messages, newMessage],
          });

          sendMessage({ userIds: openChat.participantIds });

          appContext.dispatch({
            type: "SENT_MESSAGE",
            message: newMessage,
          });
        }
      })
      .finally(() => {
        setTimeout(() => {
          setMessageIsSending(false);
        }, 800);
      });
  };

  const handleKeyDown = async (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
      await handleMessageSend();
    }
  };

  const isLastMessageFromRecipientInSeries = (message, index) => {
    if (message.senderId == user.id) return false;

    let nextMessage = openChat.messages[index + 1];

    if (!nextMessage) return true;

    if (nextMessage.senderId == user.id) return true;
    else return false;
  };

  const getMessagesGroupedByDayToMap = () => {
    return Object.values(HelperFunctions.groupMessagesByDay(openChat.messages));
  };

  const scrollMessagesToBottom = () => {
    const list = messageListRef.current;

    if (list) {
      list.scrollTop = list.scrollHeight;
    }
  };

  useEffect(() => {
    getUsers();
    getChats();

    if (selectedUser) {
      getChat(selectedUser.id);
    }
  }, [lastMessageRegistered]);

  useEffect(() => {
    scrollMessagesToBottom();
  }, [openChat?.messages?.length]);

  return (
    <div className="page chat">
      <Logo />
      <Nav />

      <Container>
        <div className="chat-container">
          <div className="user-list">
            {users?.map((u) => {
              let isActiveChat = openChat?.participantIds?.includes(u.id);
              let hasUnreadMessages = unreadChats?.some((id) => id == u.id);
              let numberOfUnreadMessages = unreadChats?.filter(
                (id) => id == u.id
              ).length;
              let unknownName = !u.firstName || !u.lastName;
              let userInitials = unknownName
                ? "??"
                : `${u.firstName[0]}${u.lastName[0]}`;
              let userName = unknownName
                ? u.email
                : `${u.firstName} ${u.lastName}`;

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
                      {numberOfUnreadMessages > 4
                        ? `+4`
                        : numberOfUnreadMessages}
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
                  <span className="user-icon">{`${selectedUser.firstName[0]}${selectedUser.lastName[0]}`}</span>
                  <span className="user-name">{`${selectedUser.firstName} ${selectedUser.lastName}`}</span>
                </div>

                <div className="message-list" ref={messageListRef}>
                  {getMessagesGroupedByDayToMap().map((ml) =>
                    ml.map((m, index) => {
                      let isMessageAuthor = m.senderId == user.id;

                      return (
                        <div
                          className={`message${
                            isMessageAuthor ? " author" : ""
                          }`}
                          key={m.id}
                        >
                          {index === 0 && (
                            <span className="message-date">
                              {HelperFunctions.getMessageTimeLabelAccordingToToday(
                                m.createdAt
                              )}
                            </span>
                          )}

                          <div className="message-box">
                            {!isMessageAuthor && (
                              <span
                                className={`message-handle${
                                  isLastMessageFromRecipientInSeries(m, index)
                                    ? " show"
                                    : ""
                                }`}
                              >{`${selectedUser.firstName[0]}${selectedUser.lastName[0]}`}</span>
                            )}
                            <div className="message-content">{m.content}</div>
                          </div>
                        </div>
                      );
                    })
                  )}
                </div>

                <div className="message-area">
                  <input
                    type="text"
                    className={`message-input${isMessageEmpty ? " empty" : ""}`}
                    placeholder="Aa"
                    ref={messageInputRef}
                    onChange={(e) => {
                      setNewMessage(e.currentTarget.value);
                      setIsMessageEmpty(false);
                    }}
                    onKeyDown={(e) => handleKeyDown(e)}
                  />
                  <span
                    className="send-icon"
                    onClick={() => handleMessageSend()}
                  >
                    {ICONS.SEND_ICON()}
                  </span>
                </div>
              </div>
            ) : (
              <span className="info-message">
                Start chatting right away by clicking on another user
              </span>
            )}
          </div>
        </div>
      </Container>

      <Footer />
    </div>
  );
};

export default Chat;
