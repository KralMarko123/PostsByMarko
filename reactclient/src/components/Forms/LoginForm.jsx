import React, { useRef } from "react";
import Button from "../Helper/Button";

const LoginForm = ({ onLogin }) => {
	const usernameRef = useRef();
	const passwordRef = useRef();

	const checkForEmptyFields = () => {
		if (usernameRef.current.value.length === 0)
			usernameRef.current.placeholder = "Please enter your username...";
		if (passwordRef.current.value.length === 0)
			passwordRef.current.placeholder = "Please enter your password...";
	};

	const handleLogin = async () => {
		if (usernameRef.current.value.length > 0 && passwordRef.current.value.length > 0) {
			const userToLogin = {
				username: usernameRef.current.value,
				password: passwordRef.current.value,
			};

			await onLogin(userToLogin);
			checkForEmptyFields();
		}
	};

	return (
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
				<Button onButtonClick={() => handleLogin()} text="Login" />
			</div>
		</form>
	);
};

export default LoginForm;
