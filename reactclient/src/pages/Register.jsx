import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import { useAuth } from "../custom/useAuth";
import { ROUTES } from "../constants/routes";
import RegisterForm from "../components/Forms/RegisterForm";
import "../styles/pages/Register.css";

const Register = () => {
	const navigate = useNavigate();
	const { user } = useAuth();

	useEffect(() => {
		if (user) navigate(ROUTES.HOME);
	}, [user]);

	return (
		<div className="register page">
			<div className="container">
				<RegisterForm />
			</div>
		</div>
	);
};

export default Register;
