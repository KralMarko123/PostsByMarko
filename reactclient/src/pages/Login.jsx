import React, { useRef } from "react";
import Button from "../components/UI/Button";
import AuthService from "../api/AuthService";
import "../styles/pages/Login.css";

const Login = () => {
	const usernameRef = useRef();
	const passwordRef = useRef();

	const onLogin = async () => {
		if (usernameRef.current.value.length > 0 && passwordRef.current.value.length > 0) {
			const userToLogin = {
				username: usernameRef.current.value,
				password: passwordRef.current.value,
			};

			await AuthService.login(userToLogin)
				.then((successfulLogin) => {
					console.log(successfulLogin.token);
				})
				.catch((error) => {
					console.error(error.message);
				});
		}

		if (usernameRef.current.value.length === 0)
			usernameRef.current.placeholder = "Please enter your username...";
		if (passwordRef.current.value.length === 0)
			passwordRef.current.placeholder = "Please enter your password...";
	};

	return (
		<div className="login page">
			<div className="container">
				<h1 className="container__title">Login</h1>
				<p className="container__description">Please use the form below to login</p>
				<form action="POST" className="form">
					<div className="form__group">
						<label htmlFor="username" className="input__label">
							Username
						</label>
						<input id="username" type="text" className="input" ref={usernameRef} />
					</div>

					<div className="form__group">
						<label htmlFor="password" className="input__label">
							Password
						</label>
						<input id="password" type="password" className="input" ref={passwordRef} />
					</div>

					<div className="form__actions">
						<Button onButtonClick={() => onLogin()} text="Login" />
					</div>
				</form>
			</div>
		</div>
	);
};

export default Login;
