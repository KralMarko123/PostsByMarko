import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import LoginForm from "../components/Forms/LoginForm";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import "../styles/pages/Login.css";

const Login = () => {
	const { user } = useAuth();
	const navigate = useNavigate();

	useEffect(() => {
		if (user) navigate(ROUTES.HOME);
	}, [user]);

	return (
		<div className="login page">
			<div className="container">
				<LoginForm />
			</div>
		</div>
	);
};

export default Login;
