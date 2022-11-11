import React, { useRef } from "react";
import Button from "../UI/Button";

const RegisterForm = ({ onRegister }) => {
	const firstNameRef = useRef();
	const lastNameRef = useRef();
	const usernameRef = useRef();
	const passwordRef = useRef();

	const checkUsername = () => {
		if (!/^\S+@\S+\.\S+$/.test(usernameRef.current.value)) {
			return false;
		} else return true;
	};

	const checkPassword = () => {
		if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/.test(passwordRef.current.value)) {
			return false;
		} else return true;
	};

	const checkForEmptyFields = () => {
		let hasEmptyFields = false;

		if (firstNameRef.current.value.length === 0) {
			firstNameRef.current.placeholder = "Please enter your first name...";
			hasEmptyFields = true;
		}
		if (lastNameRef.current.value.length === 0) {
			lastNameRef.current.placeholder = "Please enter your last name...";
			hasEmptyFields = true;
		}
		if (usernameRef.current.value.length === 0) {
			usernameRef.current.placeholder = "Please enter a username...";
			hasEmptyFields = true;
		}
		if (passwordRef.current.value.length === 0) {
			passwordRef.current.placeholder = "Please enter a password...";
			hasEmptyFields = true;
		}

		return hasEmptyFields;
	};

	const handleRegister = async () => {
		let isValidRegistration = !checkForEmptyFields() && checkUsername() && checkPassword();

		if (isValidRegistration) {
			const userToRegister = {
				firstName: firstNameRef.current.value,
				lastName: lastNameRef.current.value,
				email: usernameRef.current.value,
				userName: usernameRef.current.value,
				password: passwordRef.current.value,
			};

			await onRegister(userToRegister);
		}
	};

	return (
		<form action="POST" className="form">
			<div className="form__group">
				<label htmlFor="firstName" className="input__label">
					First Name
				</label>
				<input id="firstName" type="text" className="input" ref={firstNameRef} />
			</div>

			<div className="form__group">
				<label htmlFor="lastName" className="input__label">
					Last Name
				</label>
				<input id="lastName" type="text" className="input" ref={lastNameRef} />
			</div>

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
				<Button onButtonClick={() => handleRegister()} text="Register" />
			</div>
		</form>
	);
};

export default RegisterForm;
