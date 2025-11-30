import { createContext, useContext, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useSessionStorage } from "./useSessionStorage";
import { AuthService } from "../api/AuthService";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const navigate = useNavigate();
  const [user, setUser] = useSessionStorage("user", null);
  const isAdmin = user?.roles?.includes("Admin");

  const login = async (data) => {
    setUser(data);
    navigate(ROUTES.HOME, { replace: true });
  };

  const logout = () => {
    setUser(null);
    navigate(ROUTES.LOGIN, { replace: true });
  };

  const checkToken = async () => {
    try {
      await AuthService.validate(user.token); // token is valid, do nothing
    } catch (error) {
      logout(); // token is invalid or user does not exist, logout
    }
  };

  const value = useMemo(
    () => ({
      user,
      isAdmin,
      login,
      logout,
      checkToken,
    }),
    [user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  return useContext(AuthContext);
};
