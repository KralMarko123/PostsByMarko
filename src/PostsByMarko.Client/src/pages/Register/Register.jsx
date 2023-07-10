import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import { useAuth } from "../../custom/useAuth";
import { ROUTES } from "../../constants/routes";
import RegisterForm from "../../components/Forms/RegisterForm/RegisterForm";
import logo from "../../assets/images/POSM_icon.png";
import Card from "../../components/Helper/Card/Card";
import Container from "../../components/Layout/Container/Container";
import "../Page.css";
import "./Register.css";

const Register = () => {
	const navigate = useNavigate();
	const { user } = useAuth();

	useEffect(() => {
		if (user) navigate(ROUTES.HOME);
	}, [user]);

	return (
		<div className="register page">
			<img src={logo} className="logo" alt="posm-logo" />
			<Container>
				<Card>
					<RegisterForm />
				</Card>
			</Container>
		</div>
	);
};

export default Register;
