import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { useAuth } from "../custom/useAuth";
import { ROUTES } from "../constants/routes";
import AuthService from "../api/AuthService";
import RegisterForm from "../components/Forms/RegisterForm";
import Button from "../components/Helper/Button";
import InfoMessage from "../components/Helper/InfoMessage";
import "../styles/pages/Login.css";

const Register = () => {
	const [hasRegistered, setHasRegistered] = useState(false);
	const [isLoading, setIsLoading] = useState(false);
	const navigate = useNavigate();
	const { user } = useAuth();

	const onRegister = async (userToRegister) => {
		setIsLoading(true);

		await AuthService.register(userToRegister)
			.then(() => {
				setHasRegistered(true);
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
		<div className="register page">
			<div className="container">
				{isLoading ? (
					<InfoMessage message={"Please Wait while we register your account..."} shouldAnimate />
				) : !hasRegistered ? (
					<>
						<h1 className="container__title">Register</h1>
						<p className="container__description">Please use the form below to register</p>
						<RegisterForm onRegister={(userToRegister) => onRegister(userToRegister)} />
					</>
				) : (
					<>
						<h1 className="container__title">Successfully Registered</h1>
						<p className="container__description">Click the button below to login</p>
						<Button onButtonClick={() => navigate(ROUTES.LOGIN)} text="Login" />
					</>
				)}
			</div>
		</div>
	);
};

export default Register;
