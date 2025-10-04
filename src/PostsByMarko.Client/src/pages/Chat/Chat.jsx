import React, { useEffect, useState } from "react";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import Card from "../../components/Helper/Card/Card";
import UsersService from "../../api/UsersService";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import "../Page.css";
import "./Chat.css";

const Chat = () => {
  const [users, setUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState("");
  const { user } = useAuth();
  const { lastMessageRegistered } = useSignalR(false);

  const getUsers = async () => {
    await UsersService.getOtherUsers(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setUsers(requestResult.payload);
      }
    });
  };

  const handleUserClick = (u) => {
    setSelectedUserId(u.id);
  };

  useEffect(() => {
    getUsers();
  }, [lastMessageRegistered]);

  return (
    <div className="page chat">
      <Logo />
      <Nav />

      <Container title="Chat" desc="Stay in touch with others">
        <Card>
          <div className="chat-container">
            <div className="user-list">
              {users.map((u) => (
                <div
                  className={`user-card${
                    u.id === selectedUserId ? " active" : ""
                  }`}
                  key={u.id}
                  onClick={() => handleUserClick(u)}
                >
                  <span className="user-icon">{`${u.firstName[0]}${u.lastName[0]}`}</span>
                  <span className="user-name">{`${u.firstName} ${u.lastName}`}</span>
                </div>
              ))}
            </div>
            <div className="message-list"></div>
          </div>
        </Card>
      </Container>

      <Footer />
    </div>
  );
};

export default Chat;
