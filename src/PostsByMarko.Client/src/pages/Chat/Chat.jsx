import React, { useEffect, useRef, useState } from "react";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import Card from "../../components/Helper/Card/Card";
import UsersService from "../../api/UsersService";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import MessagingService from "../../api/MessagingService";
import { ICONS } from "../../constants/icons";
import { HelperFunctions } from "../../util/helperFunctions";
import { DateFunctions } from "../../util/dateFunctions";
import "../Page.css";
import "./Chat.css";

const Chat = () => {
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState({});
  const { user } = useAuth();
  const { lastMessageRegistered } = useSignalR(false);
  const [openChat, setOpenChat] = useState(null);
  const [newMessage, setNewMessage] = useState("");
  const [isMessageEmpty, setIsMessageEmpty] = useState(false);
  const messageInputRef = useRef(null);

  const getUsers = async () => {
    await UsersService.getOtherUsers(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setUsers(requestResult.payload);
      }
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
      }
    });
  };

  const handleUserClick = (u) => {
    setSelectedUser(u);
    getChat(u.id);
  };

  const handleMessageSend = async () => {
    if (newMessage.length === 0) {
      setIsMessageEmpty(true);
      return;
    }

    const messageToSend = {
      chatId: openChat.id,
      senderId: user.id,
      content: newMessage,
    };

    await MessagingService.sendMessage(messageToSend, user.token).then(
      (requestResult) => {
        if (requestResult.statusCode === 200) {
          const newMessage = requestResult.payload;

          let updatedChat = openChat;
          updatedChat.messages.push(newMessage);
          setOpenChat(updatedChat);

          messageInputRef.current.value = "";
          setNewMessage("");
        }
      }
    );
  };

  const handleKeyDown = async (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
      await handleMessageSend();
    }
  };

  useEffect(() => {
    getUsers();
  }, [lastMessageRegistered, openChat?.messages?.length]);

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

  return (
    <div className="page chat">
      <Logo />
      <Nav />

      <Container>
        <Card>
          <div className="chat-container">
            <div className="user-list">
              {users.map((u) => (
                <div
                  className={`user-card${u === selectedUser ? " active" : ""}`}
                  key={u.id}
                  onClick={() => handleUserClick(u)}
                >
                  <span className="user-icon">{`${u.firstName[0]}${u.lastName[0]}`}</span>
                  <span className="user-name">{`${u.firstName} ${u.lastName}`}</span>
                </div>
              ))}
            </div>
            <div className="messages">
              {openChat ? (
                <div className="message-container">
                  <div className="handle">
                    <span className="user-icon">{`${selectedUser.firstName[0]}${selectedUser.lastName[0]}`}</span>
                    <span className="user-name">{`${selectedUser.firstName} ${selectedUser.lastName}`}</span>
                  </div>

                  <div className="message-list">
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
                                {DateFunctions.getReadableDateTime(m.createdAt)}
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
                      className={`message-input${
                        isMessageEmpty ? " empty" : ""
                      }`}
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
        </Card>
      </Container>

      <Footer />
    </div>
  );
};

export default Chat;
