import React, { useState } from "react";
import Button from "../Helper/Button";
import AuthService from "../../api/AuthService";
import { useAuth } from "../../custom/useAuth";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";

const LoginForm = () => {
	const [loginData, setLoginData] = useState({
		username: "",
		password: "",
	});
	const [isLoading, setIsLoading] = useState(false);
	const [errorMessage, setErrorMessage] = useState(null);
	const { login } = useAuth();
	const navigate = useNavigate();

	const checkForEmptyFields = () => {
		let hasEmptyFields = false;

		if (loginData.username === "") {
			hasEmptyFields = true;
			setErrorMessage("Username can't be empty");
		}

		if (loginData.password === "") {
			hasEmptyFields = true;
			setErrorMessage("Password can't be empty");
		}

		return hasEmptyFields;
	};

	const handleLogin = async () => {
		let isValidLogin = !checkForEmptyFields();

		if (isValidLogin) {
			setIsLoading(true);
			await AuthService.login(loginData)
				.then((successfulLogin) => {
					login(successfulLogin);
				})
				.catch((error) => {
					error.message === "Unauthorized"
						? setErrorMessage("Invalid Login, please check your credentials and try again.")
						: setErrorMessage("Error during login, please try again later");
					console.error(error.message);
				})
				.then(() => setIsLoading(false));
		} else {
		}
	};

	return (
		<form action="POST" className="form">
			<h1 className="form__title">Login</h1>
			<div className="form__group">
				<label htmlFor="username" className="input__label">
					Username
				</label>
				<input
					id="username"
					type="text"
					className="input"
					placeholder="Enter your username"
					onChange={(e) => setLoginData({ ...loginData, username: e.currentTarget.value })}
				/>
			</div>

			<div className="form__group">
				<label htmlFor="password" className="input__label">
					Password
				</label>
				<input
					id="password"
					type="password"
					className="input"
					placeholder="Enter your password"
					onChange={(e) => setLoginData({ ...loginData, password: e.currentTarget.value })}
				/>
			</div>

			<div className="form__actions">
				<Button onButtonClick={() => handleLogin()} text="Login" loading={isLoading} />
			</div>

			<p className="register__link" onClick={() => navigate(ROUTES.REGISTER)}>
				Haven't registered yet? Click here to create an account
			</p>

			{errorMessage && <p className="error__message">{errorMessage}</p>}
		</form>
	);
};

export default LoginForm;
