import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import LoginForm from "../../components/Forms/LoginForm/LoginForm";
import { ROUTES } from "../../constants/routes";
import { useAuth } from "../../custom/useAuth";
import logo from "../../assets/images/POSM_icon.png";
import Card from "../../components/Helper/Card/Card";
import "../Page.css";
import "./Login.css";

const Login = () => {
	const { user } = useAuth();
	const navigate = useNavigate();

	useEffect(() => {
		if (user) navigate(ROUTES.HOME);
	}, [user]);

	return (
		<div className="login page">
			<img src={logo} className="logo" alt="posm-logo" />
			<div className="container">
				<Card>
					<LoginForm />
				</Card>
			</div>
		</div>
	);
};

export default Login;
