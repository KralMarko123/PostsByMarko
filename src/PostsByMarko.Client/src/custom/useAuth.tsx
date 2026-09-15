import { HttpError } from "../api/ApiClient";
import { createContext, useContext, useMemo, useRef, useCallback } from "react";
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
  const currentToken = useRef(user?.token);
  currentToken.current = user?.token;
  const isAdmin = user?.roles?.includes("Admin") ?? false;

  const login = useCallback(async (user: AuthUser) => {
    setUser(user);
    navigate(ROUTES.HOME, { replace: true });
  }, [setUser, navigate]);

  const logout = useCallback(() => {
    setUser(null);
    navigate(ROUTES.LOGIN, { replace: true });
  }, [setUser, navigate]);

  const checkToken = useCallback(async () => {
    const token = user?.token;
    if (!token) return;
    try {
      await AuthService.validate(token);
    } catch (error) {
      if (currentToken.current === token && error instanceof HttpError && error.status === 401) logout();
    }
  }, [user?.token, logout]);

  const value = useMemo(
    () => ({
      user,
      isAdmin,
      login,
      logout,
      checkToken,
    }),
    [user, isAdmin, login, logout, checkToken]
  );

  return <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>;
};

export function useAuth(): AuthContextValue {
  return useContext(AuthContext);
}
