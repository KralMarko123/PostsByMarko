import React, { useContext, useEffect, useMemo, useRef, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/icons";
import { HelperFunctions } from "../../util/helperFunctions";
import { DateFunctions } from "../../util/dateFunctions";
import { useMessageHub } from "../../custom/useMessageHub";
import { UserService } from "../../api/UserService";
import { MessagingService } from "../../api/MessagingService";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import AppContext from "../../context/AppContext";
import "../Page.css";
import "./Chat.css";

const Chat = () => {
  const appContext = useContext(AppContext);
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState(null);
  const [unreadChats, setUnreadChats] = useState([]);
  const [openChat, setOpenChat] = useState(null);
  const { user, checkToken } = useAuth();
  const { lastMessageRegistered } = useMessageHub();
  const [newMessage, setNewMessage] = useState("");
  const [isMessageEmpty, setIsMessageEmpty] = useState(false);
  const messageInputRef = useRef(null);
  const messageListRef = useRef(null);
  const [messageIsSending, setMessageIsSending] = useState(false);

  const getUsers = async () => {
    await UserService.getUsers(user.token, user.id)
      .then((users) => {
        setUsers(users);
      })
      .catch(async (error) => {
        await checkToken();

        // TODO: Create global notification modal
        console.log(error);
      });
  };

  const getChats = async () => {
    await MessagingService.getChats(user.token)
      .then((chatsResponse) => {
        appContext.chats.forEach((localChat) => {
          let existingUnopenedChat = chatsResponse.find(
            (c) => c.id === localChat.id && c.id !== openChat?.id
          );

          if (
            existingUnopenedChat &&
            existingUnopenedChat.messages.length > localChat.messages.length
          ) {
            let userIdWithNewMessages = existingUnopenedChat.users.find(
              (u) => u.id !== user.id
            ).id;

            console.log(userIdWithNewMessages);

            setUnreadChats([...unreadChats, userIdWithNewMessages]);
          }
        });

        appContext.dispatch({ type: "LOAD_CHATS", chats: chatsResponse });
      })
      .catch(async (error) => {
        await checkToken();

        // TODO: Create global notification modal
        console.log(error);
      });
  };

  const startChat = async (recipientId) => {
    await MessagingService.startChat(recipientId, user.token)
      .then((chat) => {
        setOpenChat(chat);

        appContext.dispatch({
          type: "STARTED_CHAT",
          chat: chat,
        });
      })
      .catch((error) => {
        // TODO: Create global notification modal
        console.log(error);
      });
  };

  const handleUserClick = (u) => {
    if (newMessage.length > 0 && selectedUser !== u) {
      setNewMessage("");
      messageInputRef.current.value = "";
    }

    setSelectedUser(u);
    startChat(u.id);
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
      .then((newMessage) => {
        messageInputRef.current.value = "";
        setNewMessage("");

        setOpenChat({
          ...openChat,
          messages: [...openChat.messages, newMessage],
        });

        appContext.dispatch({
          type: "SENT_MESSAGE",
          message: newMessage,
        });
      })
      .catch((error) => {
        // TODO: Create global notification modal
        console.log(error);
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

  const isLastMessageFromRecipientInSeries = (message, messages) => {
    if (messages.length === 0) return false;

    let otherUserMessages = messages.filter((m) => m.senderId !== user.id);

    DateFunctions.sortItemsByDateTimeAttribute(otherUserMessages, "createdAt");

    if (otherUserMessages[otherUserMessages.length - 1].id === message.id)
      return true;
    else return false;
  };

  const getMessagesGroupedByDay = () => {
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
      startChat(selectedUser.id);
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
              let isActiveChat = openChat?.users
                ?.map((cu) => cu.id)
                ?.includes(u.id);
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
                  {getMessagesGroupedByDay().map((messageList) => {
                    DateFunctions.sortItemsByDateTimeAttribute(
                      messageList,
                      "createdAt"
                    );

                    return messageList.map((m, index) => {
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
                                  isLastMessageFromRecipientInSeries(
                                    m,
                                    messageList
                                  )
                                    ? " show"
                                    : ""
                                }`}
                              >{`${selectedUser.firstName[0]}${selectedUser.lastName[0]}`}</span>
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
