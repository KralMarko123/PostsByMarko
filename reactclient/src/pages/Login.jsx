import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import AuthService from "../api/AuthService";
import LoginForm from "../components/Forms/LoginForm";
import InfoMessage from "../components/UI/InfoMessage";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import "../styles/pages/Login.css";

const Login = () => {
	const [isLoading, setIsLoading] = useState(false);
	const { user, login } = useAuth();
	const navigate = useNavigate();

	const onLogin = async (userToLogin) => {
		setIsLoading(true);
		await AuthService.login(userToLogin)
			.then((successfulLogin) => {
				login(successfulLogin);
			})
			.catch((error) => {
				console.error(error.message);
			})
			.then(() => setIsLoading(false));
	};

	useEffect(() => {
		if (user) navigate(ROUTES.HOME);
	}, [user]);

	return (
		<div className="login page">
			<div className="container">
				{isLoading ? (
					<InfoMessage message={"Please Wait while we log you in..."} shouldAnimate />
				) : (
					<>
						<h1 className="container__title">Login</h1>
						<p className="container__description">Please use the form below to login</p>
						<LoginForm onLogin={(userToLogin) => onLogin(userToLogin)} />
					</>
				)}
			</div>
		</div>
	);
};

export default Login;
