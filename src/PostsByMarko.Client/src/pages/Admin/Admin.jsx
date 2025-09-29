import React, { useContext, useEffect, useState } from "react";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
import Nav from "../../components/Layout/Nav/Nav";
import UsersService from "../../api/UsersService";
import { useAuth } from "../../custom/useAuth";
import { HelperFunctions } from "../../util/helperFunctions";
import { useSignalR } from "../../custom/useSignalR";
import AppContext from "../../context/AppContext";
import Card from "../../components/Helper/Card/Card";
import { ROUTES } from "../../constants/routes";
import { useNavigate } from "react-router";
import "../Page.css";
import "./Admin.css";

const Admin = () => {
  const appContext = useContext(AppContext);
  const { user, isAdmin } = useAuth();
  const { lastMessageRegistered } = useSignalR();
  const [data, setData] = useState([]);
  const [errorMessage, setErrorMessage] = useState(null);
  const navigate = useNavigate();

  const getAdminDashboard = async () => {
    await UsersService.GetAdminDashboard(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setErrorMessage(null);
        setData(requestResult.payload);
      } else setErrorMessage(requestResult.message);
    });
  };

  useEffect(() => {
    getAdminDashboard();
  }, [lastMessageRegistered, appContext.posts.length]);

  return (
    <div className="admin page">
      <img
        src={logo}
        className="logo"
        alt="posm-logo"
        onClick={() => navigate(ROUTES.HOME)}
      />
      <Nav />

      <Container
        title="Admin Dashboard"
        desc="Manage users and view statistics"
      >
        {errorMessage && <p className="error">{errorMessage}</p>}

        <Card buttonRadius>
          <table className="table">
            <thead>
              <tr>
                <th scope="col">User</th>
                <th scope="col">Number of Posts</th>
                <th scope="col">Last Posted</th>
                <th scope="col">Roles</th>
                <th scope="col">Actions</th>
              </tr>
            </thead>

            <tbody>
              {data.map((d, index) => (
                <tr key={d.id} className={index % 2 != 0 ? "odd" : ""}>
                  <td>{d.email}</td>
                  <td>{d.numberOfPosts}</td>
                  <td>
                    {d.lastPostedAt
                      ? HelperFunctions.getReadablePostDate(d.lastPostedAt)
                      : ""}
                  </td>
                  <td>
                    {d.roles.map((r) => (
                      <span className="table-badge">{r}</span>
                    ))}
                  </td>
                  <td>
                    <span className="table-button error">Delete</span>
                    <span className="table-button success">Make Admin</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      </Container>
    </div>
  );
};

export default Admin;
