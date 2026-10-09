import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";
import { useContext, useEffect, useState } from "react";
import { AdminService } from "../../api/AdminService";
import { PostService } from "../../api/PostService";
import { useAuth } from "../../custom/useAuth";
import { DateFunctions } from "../../util/dateFunctions";
import { ActionType } from "../../constants/enums";
import { Container } from "../../components/Layout/Container/Container";
import { AppContext } from "../../context/AppContext";
import { Card } from "../../components/Helper/Card/Card";
import { BarChart } from "../../components/Charts/BarChart";
import { Post } from "@typeConfigs/post";
import { AdminDashboardResponse } from "@typeConfigs/admin";

export const Admin = () => {
  const appContext = useContext(AppContext);
  const { user, checkToken } = useAuth();
  const [dashboardData, setDashboardData] = useState<AdminDashboardResponse[]>([]);
  const [posts, setPosts] = useState<Post[]>([]);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState("");

  const barChartLabels = [...Array(DateFunctions.getCurrentMonthDayNumber()).keys()].map((i) =>
    (i + 1).toString(),
  );
  const barChartData = DateFunctions.countPostsByDay(
    posts.flatMap((post) => (post.createdAt ? [post.createdAt] : [])),
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
        checkToken();

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
    const updateRolesRequest = {
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
  }, [appContext.lastMessageRegistered, appContext.lastAdminAction, appContext.posts.length]);

  return (
    <PageLayout className="admin">
      <Container title="Admin Dashboard" desc="Manage users and view statistics">
        {dashboardData.length > 0 ? (
          <Card className="overflow-hidden">
            <p className="border-b border-line/60 px-5 py-3 text-xs text-muted xl:hidden">
              Scroll sideways to see all user details and actions.
            </p>
            <div className="overflow-x-auto">
              <table
                className="w-full min-w-240 border-collapse text-sm"
                aria-label="User management"
              >
                <thead className="bg-elevated text-muted [&_th]:px-5 [&_th]:py-4 [&_th]:text-left [&_th]:font-semibold [&_th]:whitespace-nowrap">
                  <tr>
                    <th scope="col">User</th>
                    <th scope="col">Number of Posts</th>
                    <th scope="col">Last Posted</th>
                    <th scope="col">Roles</th>
                    <th scope="col">Actions</th>
                  </tr>
                </thead>

                <tbody className="divide-y divide-line/60 [&_td]:px-5 [&_td]:py-4">
                  {dashboardData.map((row) => (
                    <tr key={row.userId} className="hover:bg-elevated/50" data-email={row.email}>
                      <td>{row.email}</td>
                      <td>{row.numberOfPosts}</td>
                      <td>
                        {row.lastPostedAt
                          ? DateFunctions.getReadableDateTime(row.lastPostedAt)
                          : ""}
                      </td>
                      <td>
                        {row.roles.map((r) => (
                          <span
                            className="table-badge mr-1 inline-block rounded-full border border-line px-2.5 py-1 text-xs text-mint"
                            key={r}
                          >
                            {r}
                          </span>
                        ))}
                      </td>
                      <td>
                        <div className="flex gap-2 whitespace-nowrap">
                          {row.roles.includes("Admin") ? (
                            <button
                              type="button"
                              className="table-button warning min-h-11 rounded-lg border border-line px-3 py-2 text-warning hover:bg-elevated"
                              onClick={async () => await handleUserRoleUpdate(row.userId, false)}
                            >
                              Remove Admin
                            </button>
                          ) : (
                            <button
                              type="button"
                              className="table-button success min-h-11 rounded-lg border border-line px-3 py-2 text-positive hover:bg-elevated"
                              onClick={async () => await handleUserRoleUpdate(row.userId, true)}
                            >
                              Make Admin
                            </button>
                          )}
                          <button
                            type="button"
                            className="table-button error min-h-11 rounded-lg border border-line px-3 py-2 text-danger hover:bg-danger-surface"
                            onClick={async () => await handleUserDelete(row.userId, row.email)}
                          >
                            Delete
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Card>
        ) : (
          <p className="info">No Users Present</p>
        )}

        <div className="my-5">
          {errorMessage && (
            <p className="error" role="alert">
              {errorMessage}
            </p>
          )}
          {confirmationalMessage && (
            <p className="success" role="status">
              {confirmationalMessage}
            </p>
          )}
        </div>

        <div className="charts-container mt-8 rounded-2xl border border-line/70 bg-surface p-4 sm:p-6">
          <div className="chart relative h-80">
            <BarChart title={"Posts this month"} labels={barChartLabels} data={barChartData} />
          </div>
        </div>
      </Container>
    </PageLayout>
  );
};
