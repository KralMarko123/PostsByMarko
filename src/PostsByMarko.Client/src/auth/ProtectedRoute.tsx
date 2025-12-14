import { Navigate } from "react-router-dom";
import { useAuth } from "../custom/useAuth";
import { ROUTES } from "../constants/routes";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export const ProtectedRoute = (props: ProtectedRouteProps) => {
  const { user } = useAuth();

  if (!user || !user.email || !user.token) {
    return <Navigate to={ROUTES.LOGIN} />;
  }

  return <>{props.children}</>;
};
