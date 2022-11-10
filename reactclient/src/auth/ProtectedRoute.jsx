import { Navigate } from "react-router-dom";
import { useAuth } from "../custom/useAuth";
import { ROUTES } from "../constants/routes";

export const ProtectedRoute = ({ children }) => {
	const { user } = useAuth();

	if (!user) return <Navigate to={ROUTES.LOGIN} />;
	return children;
};
