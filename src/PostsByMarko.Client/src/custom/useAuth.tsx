import { createContext, useContext, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useSessionStorage } from "./useSessionStorage";
import { AuthService } from "../api/AuthService";
import { AuthContextValue, AuthProviderProps, AuthUser } from "types/auth";

const defaultAuthContext: AuthContextValue = {
  user: null,
  isAdmin: false,
  login: () => null,
  logout: () => null,
  checkToken: () => null,
};
const AuthContext = createContext<AuthContextValue>(defaultAuthContext);
const STORAGE_KEY = "authenticated_user";

export const AuthProvider = (props: AuthProviderProps) => {
  const navigate = useNavigate();
  const [user, setUser] = useSessionStorage<AuthUser | null>(STORAGE_KEY, null);
  const isAdmin = user?.roles?.includes("Admin") ?? false;

  const login = async (user: AuthUser) => {
    setUser(user);
    navigate(ROUTES.HOME, { replace: true });
  };

  const logout = () => {
    setUser(null);
    navigate(ROUTES.LOGIN, { replace: true });
  };

  const checkToken = async () => {
    try {
      await AuthService.validate(user!.token!); // token is valid, do nothing
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

  return <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>;
};

export function useAuth(): AuthContextValue {
  return useContext(AuthContext);
}
