import React, { useContext, useEffect, useState } from "react";
import Container from "../../components/Layout/Container/Container";
import Nav from "../../components/Layout/Nav/Nav";
import UserService from "../../api/UserService";
import PostService from "../../api/PostService";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import AppContext from "../../context/AppContext";
import Card from "../../components/Helper/Card/Card";
import { useNavigate } from "react-router";
import BarChart from "../../components/Charts/BarChart";
import LineChart from "../../components/Charts/LineChart";
import Footer from "../../components/Layout/Footer/Footer";
import { DateFunctions } from "../../util/dateFunctions";
import Logo from "../../components/Layout/Logo/Logo";
import "../Page.css";
import "./Admin.css";

const Admin = () => {
  const appContext = useContext(AppContext);
  const { user } = useAuth();
  const { sendMessage, lastMessageRegistered } = useSignalR();
  const [users, setUsers] = useState([]);
  const [posts, setPosts] = useState([]);
  const [errorMessage, setErrorMessage] = useState("");
  const [confirmationalMessage, setConfirmationalMessage] = useState("");
  const navigate = useNavigate();

  const barChartLabels = [
    ...Array(DateFunctions.getCurrentMonthDayNumber()).keys(),
  ].map((i) => i + 1);
  const barChartData = barChartLabels.map(
    (l) =>
      posts.filter(
        (p) =>
          DateFunctions.getDayOfMonthFromDate(p.createdDate) === l.toString()
      ).length
  );
  const lineChartLabels = DateFunctions.getThisMonthsDates();
  const lineChartData = lineChartLabels.map(
    (l) =>
      users.filter(
        (u) => DateFunctions.getDateTimeInFormat(u.createdAt, "D/M/YYYY") === l
      ).length
  );

  const getAdminDashboard = async () => {
    await UserService.getAdminDashboard(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setErrorMessage(null);
        setUsers(requestResult.payload);
      } else if (requestResult.statusCode === 204) {
        setUsers([]);
      } else setErrorMessage(requestResult.message);
    });
  };

  const getPosts = async () => {
    await PostService.getPosts(user.token).then((requestResult) => {
      setPosts(requestResult.payload);
      appContext.dispatch({ type: "LOAD_POSTS", posts: requestResult.payload });
    });
  };

  const handleUserDelete = async (userId, userEmail) => {
    await UsersService.DeleteUser(userId, user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        sendMessage("Deleted User");
        setConfirmationalMessage(`User ${userEmail} removed successfully`);
        setErrorMessage(null);
        setTimeout(() => setConfirmationalMessage(null), 3000);
      } else {
        setErrorMessage(`Error while removing user ${userEmail}`);
        setConfirmationalMessage(null);
      }
    });
  };

  const handleUserMadeAdmin = async (userId) => {
    await UsersService.addRoleToUser(userId, "Admin", user.token).then(
      (requestResult) => {
        if (requestResult.statusCode === 200) {
          sendMessage("Updated User");
          setConfirmationalMessage(requestResult.message);
          setErrorMessage(null);
          setTimeout(() => setConfirmationalMessage(null), 3000);
        } else {
          setErrorMessage(requestResult.message);
          setConfirmationalMessage(null);
        }
      }
    );
  };

  const handleUserRemovedAdmin = async (userId) => {
    await UsersService.removeRoleFromUser(userId, "Admin", user.token).then(
      (requestResult) => {
        if (requestResult.statusCode === 200) {
          sendMessage("Updated User");
          setConfirmationalMessage(requestResult.message);
          setErrorMessage(null);
          setTimeout(() => setConfirmationalMessage(null), 3000);
        } else {
          setErrorMessage(requestResult.message);
          setConfirmationalMessage(null);
        }
      }
    );
  };

  useEffect(() => {
    getAdminDashboard();
    getPosts();
  }, [lastMessageRegistered, appContext.posts.length]);

  return (
    <div className="admin page">
      <Logo />
      <Nav />

      <Container
        title="Admin Dashboard"
        desc="Manage users and view statistics"
      >
        {users.length > 0 ? (
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
                {users.map((user, index) => (
                  <tr
                    key={user.id}
                    className={index % 2 != 0 ? "odd" : ""}
                    data-email={user.email}
                  >
                    <td>{user.email}</td>
                    <td>{user.numberOfPosts}</td>
                    <td>
                      {user.lastPostedAt
                        ? DateFunctions.getReadableDateTime(user.lastPostedAt)
                        : ""}
                    </td>
                    <td>
                      {user.roles.map((r) => (
                        <span className="table-badge" key={r}>
                          {r}
                        </span>
                      ))}
                    </td>
                    <td>
                      <span
                        className="table-button error"
                        onClick={() => handleUserDelete(user.id, user.email)}
                      >
                        Delete
                      </span>
                      {user.roles.includes("Admin") ? (
                        <span
                          className="table-button warning"
                          onClick={() => handleUserRemovedAdmin(user.id)}
                        >
                          Remove Admin
                        </span>
                      ) : (
                        <span
                          className="table-button success"
                          onClick={() => handleUserMadeAdmin(user.id)}
                        >
                          Make Admin
                        </span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Card>
        ) : (
          <p className="info">No Users Present</p>
        )}

        <div className="messages">
          {errorMessage && <p className="error">{errorMessage}</p>}
          {confirmationalMessage && (
            <p className="success">{confirmationalMessage}</p>
          )}
        </div>

        <div className="charts-container">
          <div className="chart">
            <BarChart
              title={"Posts this month"}
              labels={barChartLabels}
              data={barChartData}
            />
          </div>

          <div className="chart">
            <LineChart
              title={"Registered users this month"}
              labels={lineChartLabels}
              data={lineChartData}
            />
          </div>
        </div>
      </Container>

      <Footer />
    </div>
  );
};

export default Admin;
