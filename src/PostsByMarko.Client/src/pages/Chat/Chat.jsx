import React, { useEffect, useState } from "react";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import Card from "../../components/Helper/Card/Card";
import UsersService from "../../api/UsersService";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import MessagingService from "../../api/MessagingService";
import "../Page.css";
import "./Chat.css";
import { ICONS } from "../../constants/icons";

const Chat = () => {
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState({});
  const { user } = useAuth();
  const { lastMessageRegistered } = useSignalR(false);
  const [openChat, setOpenChat] = useState(null);
  const [newMessage, setNewMessage] = useState("");
  const [isMessageEmpty, setIsMessageEmpty] = useState(false);

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

  const handleMessageSend = () => {
    if (newMessage.length === 0) {
      console.log("here");
      setIsMessageEmpty(true);
      return;
    }
  };

  useEffect(() => {
    getUsers();
  }, [lastMessageRegistered]);

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
                    {openChat.messages.map((m) => {
                      <div className="message">{m}</div>;
                    })}
                  </div>

                  <div className="message-area">
                    <input
                      type="text"
                      className={`message-input${
                        isMessageEmpty ? " empty" : ""
                      }`}
                      placeholder="Aa"
                      onChange={(e) => {
                        setNewMessage(e.currentTarget.value);
                        setIsMessageEmpty(false);
                      }}
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
