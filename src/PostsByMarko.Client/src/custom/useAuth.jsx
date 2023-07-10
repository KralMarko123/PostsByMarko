import { createContext, useContext, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useSessionStorage } from "./useSessionStorage";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
	const [user, setUser] = useSessionStorage("user", null);
	const navigate = useNavigate();
	const isAdmin = user?.roles.includes("Admin");
	const isEditor = user?.roles.includes("Editor");

	const login = async (data) => {
		setUser(data);
		navigate(ROUTES.HOME, { replace: true });
	};

	const logout = () => {
		setUser(null);
		navigate(ROUTES.LOGIN, { replace: true });
	};

	const value = useMemo(
		() => ({
			user,
			isAdmin,
			isEditor,
			login,
			logout,
		}),
		[user]
	);

	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
	return useContext(AuthContext);
};
