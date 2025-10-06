import React from "react";
import { Routes, Route } from "react-router-dom";
import { ROUTES } from "./constants/routes";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import Home from "./pages/Home/Home";
import Details from "./pages/Details/Details";
import Login from "./pages/Login/Login";
import Register from "./pages/Register/Register";
import AppProvider from "./context/ContextProvider";
import Admin from "./pages/Admin/Admin";
import Chat from "./pages/Chat/Chat";
import NotFound from "./pages/NotFound/NotFound";

const App = () => {
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
              <Chat />
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

export default App;
