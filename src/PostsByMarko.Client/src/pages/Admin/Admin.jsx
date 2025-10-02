import React, { useContext, useEffect, useState } from "react";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
import Nav from "../../components/Layout/Nav/Nav";
import UsersService from "../../api/UsersService";
import PostsService from "../../api/PostsService";
import { useAuth } from "../../custom/useAuth";
import { HelperFunctions } from "../../util/helperFunctions";
import { useSignalR } from "../../custom/useSignalR";
import AppContext from "../../context/AppContext";
import Card from "../../components/Helper/Card/Card";
import { ROUTES } from "../../constants/routes";
import { useNavigate } from "react-router";
import BarChart from "../../components/Charts/BarChart";
import LineChart from "../../components/Charts/LineChart";
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
    ...Array(HelperFunctions.getCurrentMonthDayNumber()).keys(),
  ].map((i) => i + 1);
  const barChartData = barChartLabels.map(
    (l) =>
      posts.filter(
        (p) =>
          HelperFunctions.getDayOfMonthFromDate(p.createdDate) === l.toString()
      ).length
  );

  const lineChartLabels = HelperFunctions.getThisMonthsDates();
  console.log(lineChartLabels);

  const lineChartData = lineChartLabels.map(
    (l) =>
      users.filter(
        (u) =>
          HelperFunctions.getDateTimeInFormat(u.createdAt, "D/M/YYYY") === l
      ).length
  );
  console.log(lineChartData);

  const getAdminDashboard = async () => {
    await UsersService.GetAdminDashboard(user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setErrorMessage(null);
        setUsers(requestResult.payload);
      } else if (requestResult.statusCode === 204) {
        setUsers([]);
      } else setErrorMessage(requestResult.message);
    });
  };

  const getPosts = async () => {
    await PostsService.getAllPosts(user.token).then((requestResult) => {
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
                  <tr key={user.id} className={index % 2 != 0 ? "odd" : ""}>
                    <td>{user.email}</td>
                    <td>{user.numberOfPosts}</td>
                    <td>
                      {user.lastPostedAt
                        ? HelperFunctions.getReadablePostDate(user.lastPostedAt)
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

        <div className="chart-container">
          {/* <BarChart
            title={"Posts this month"}
            labels={chartLabels}
            data={chartData}
          /> */}

          <LineChart
            title={"Registered users this month"}
            labels={lineChartLabels}
            data={lineChartData}
          />
        </div>
      </Container>
    </div>
  );
};

export default Admin;
