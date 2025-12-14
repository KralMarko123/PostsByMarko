import { useContext, useEffect, useState } from "react";
import { AdminService } from "../../api/AdminService";
import { PostService } from "../../api/PostService";
import { useAuth } from "../../custom/useAuth";
import { DateFunctions } from "../../util/dateFunctions";
import { ActionType } from "../../constants/enums";
import { usePostHub } from "../../custom/usePostHub";
import { useAdminHub } from "../../custom/useAdminHub";
import { Nav } from "../../components/Layout/Nav/Nav";
import { Container } from "../../components/Layout/Container/Container";
import { AppContext } from "../../context/AppContext";
import { Card } from "../../components/Helper/Card/Card";
import { BarChart } from "../../components/Charts/BarChart";
import { Footer } from "../../components/Layout/Footer/Footer";
import { Logo } from "../../components/Layout/Logo/Logo";
import { Post } from "@typeConfigs/post";
import { AdminDashboardResponse } from "@typeConfigs/admin";
import "./Admin.css";
import "../Page.css";

export const Admin = () => {
  const appContext = useContext(AppContext);
  const { user, checkToken } = useAuth();
  const lastMessageRegistered = usePostHub();
  const lastAdminAction = useAdminHub();
  const [dashboardData, setDashboardData] = useState<AdminDashboardResponse[]>([]);
  const [posts, setPosts] = useState<Post[]>([]);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState("");

  const barChartLabels = [...Array(DateFunctions.getCurrentMonthDayNumber()).keys()].map(
    (i) => (i + 1).toString()
  );
  const barChartData = barChartLabels.map(
    (l) =>
      posts.filter(
        (p) => DateFunctions.getDayOfMonthFromDate(p.createdAt!) === l.toString()
      ).length
  );

  const getAdminDashboard = async () => {
    await AdminService.getDashboard(user!.token!)
      .then((data) => {
        setErrorMessage("");
        setDashboardData(data);
      })
      .catch((error) => setErrorMessage(error.message));
  };

  const getPosts = async () => {
    await PostService.getPosts(user!.token!)
      .then((posts) => {
        setPosts(posts);
        appContext.dispatch({ type: "LOAD_POSTS", posts: posts });
      })
      .catch(async (error) => {
        await checkToken();

        // TODO: Setup some global notification modal showing error
        console.log(error);
      });
  };

  const handleUserDelete = async (userId: string, userEmail: string) => {
    await AdminService.deleteUser(userId, user!.token!)
      .then(() => {
        setConfirmationalMessage(`User ${userEmail} removed successfully`);
        setErrorMessage("");
        setTimeout(() => setConfirmationalMessage(""), 3000);
      })
      .catch((error) => {
        setConfirmationalMessage("");
        setErrorMessage(error.message);
      });
  };

  const handleUserRoleUpdate = async (userId: string, addRole: boolean = true) => {
    let updateRolesRequest = {
      userId: userId,
      actionType: addRole ? ActionType.CREATE : ActionType.DELETE,
      role: "Admin",
    };

    await AdminService.updateUserRoles(updateRolesRequest, user!.token!)
      .then(() => {
        setConfirmationalMessage("User roles updated successfully!");
        setErrorMessage("");
        setTimeout(() => setConfirmationalMessage(""), 3000);
      })
      .catch((error) => {
        setErrorMessage(error.message);
        setConfirmationalMessage("");
      });
  };

  useEffect(() => {
    getAdminDashboard();
    getPosts();
  }, [lastAdminAction, lastMessageRegistered, appContext.posts.length]);

  return (
    <div className="admin page">
      <Logo />
      <Nav />

      <Container title="Admin Dashboard" desc="Manage users and view statistics">
        {dashboardData.length > 0 ? (
          <Card>
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
                {dashboardData.map((row, index) => (
                  <tr
                    key={row.userId}
                    className={index % 2 != 0 ? "odd" : ""}
                    data-email={row.email}
                  >
                    <td>{row.email}</td>
                    <td>{row.numberOfPosts}</td>
                    <td>
                      {row.lastPostedAt
                        ? DateFunctions.getReadableDateTime(row.lastPostedAt)
                        : ""}
                    </td>
                    <td>
                      {row.roles.map((r) => (
                        <span className="table-badge" key={r}>
                          {r}
                        </span>
                      ))}
                    </td>
                    <td>
                      {row.roles.includes("Admin") ? (
                        <span
                          className="table-button warning"
                          onClick={async () =>
                            await handleUserRoleUpdate(row.userId, false)
                          }
                        >
                          Remove Admin
                        </span>
                      ) : (
                        <span
                          className="table-button success"
                          onClick={async () =>
                            await handleUserRoleUpdate(row.userId, true)
                          }
                        >
                          Make Admin
                        </span>
                      )}
                      <span
                        className="table-button error"
                        onClick={async () =>
                          await handleUserDelete(row.userId, row.email)
                        }
                      >
                        Delete
                      </span>
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
          {confirmationalMessage && <p className="success">{confirmationalMessage}</p>}
        </div>

        <div className="charts-container">
          <div className="chart">
            <BarChart
              title={"Posts this month"}
              labels={barChartLabels}
              data={barChartData}
            />
          </div>
        </div>
      </Container>

      <Footer />
    </div>
  );
};
