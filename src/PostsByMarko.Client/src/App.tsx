import { Routes, Route } from "react-router-dom";
import { ROUTES } from "./constants/routes";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { Home } from "./pages/Home/Home";
import { Details } from "./pages/Details/Details";
import { Login } from "./pages/Login/Login";
import { Register } from "./pages/Register/Register";
import { AppProvider } from "./context/AppProvider";
import { Admin } from "./pages/Admin/Admin";
import { Chats } from "./pages/Chats/Chats";
import { NotFound } from "./pages/NotFound/NotFound";

export const App = () => {
  return (
    <AppProvider>
      <Routes>
        <Route
          path={ROUTES.HOME}
          element={
            <ProtectedRoute>
              <Home />
            </ProtectedRoute>
          }
        />
        <Route
          path={ROUTES.DETAILS}
          element={
            <ProtectedRoute>
              <Details />
            </ProtectedRoute>
          }
        />
        <Route
          path={ROUTES.ADMIN}
          element={
            <ProtectedRoute>
              <Admin />
            </ProtectedRoute>
          }
        />
        <Route
          path={ROUTES.CHAT}
          element={
            <ProtectedRoute>
              <Chats />
            </ProtectedRoute>
          }
        />
        <Route path={ROUTES.LOGIN} element={<Login />} />
        <Route path={ROUTES.REGISTER} element={<Register />} />
        <Route path={ROUTES.NOT_FOUND} element={<NotFound />} />
      </Routes>
    </AppProvider>
  );
};
