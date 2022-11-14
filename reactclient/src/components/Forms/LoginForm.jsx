import React, { useRef } from "react";
import Button from "../Helper/Button";

const LoginForm = ({ onLogin }) => {
	const usernameRef = useRef();
	const passwordRef = useRef();

	const checkForEmptyFields = () => {
		let hasEmptyFields = false;

		if (usernameRef.current.value.length === 0) {
			usernameRef.current.placeholder = "Please enter your username...";
			hasEmptyFields = true;
		}
		if (passwordRef.current.value.length === 0) {
			passwordRef.current.placeholder = "Please enter your password...";
			hasEmptyFields = true;
		}

		return hasEmptyFields;
	};

	const handleLogin = async () => {
		let isValidLogin = !checkForEmptyFields();
		if (isValidLogin) {
			const userToLogin = {
				username: usernameRef.current.value,
				password: passwordRef.current.value,
			};

			await onLogin(userToLogin);
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
