import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import LoginForm from "../../components/Forms/LoginForm/LoginForm";
import { ROUTES } from "../../constants/routes";
import { useAuth } from "../../custom/useAuth";
import logo from "../../assets/images/POSM_icon.png";
import Card from "../../components/Helper/Card/Card";
import Container from "../../components/Layout/Container/Container";
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
			<Container>
				<Card>
					<LoginForm />
				</Card>
			</Container>
		</div>
	);
};

export default Login;
