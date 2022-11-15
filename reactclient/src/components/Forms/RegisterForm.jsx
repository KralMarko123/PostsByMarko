import React, { useState } from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";
import Button from "../Helper/Button";
import AuthService from "../../api/AuthService";

const RegisterForm = () => {
	const [registerData, setRegisterData] = useState({
		firstName: "",
		lastName: "",
		username: "",
		password: "",
		confirmPassword: "",
	});
	const [isLoading, setIsLoading] = useState(false);
	const [errorMessage, setErrorMessage] = useState(null);
	const [hasRegistered, setHasRegistered] = useState(false);
	const navigate = useNavigate();

	const checkForEmptyFields = () => {
		let hasEmptyFields = false;

		if (registerData.firstName === "") {
			hasEmptyFields = true;
			setErrorMessage("First Name can't be empty");
		}

		if (registerData.lastName === "") {
			hasEmptyFields = true;
			setErrorMessage("Last Name can't be empty");
		}

		if (registerData.username === "") {
			hasEmptyFields = true;
			setErrorMessage("Username can't be empty");
		}

		if (registerData.password === "") {
			hasEmptyFields = true;
			setErrorMessage("Password can't be empty");
		}

		if (registerData.confirmPassword === "") {
			hasEmptyFields = true;
			setErrorMessage("Confirm Password can't be empty");
		}

		return hasEmptyFields;
	};

	const checkUsername = () => {
		if (!/^\S+@\S+\.\S+$/.test(registerData.username)) {
			setErrorMessage("Username is invalid");
			return false;
		} else return true;
	};

	const checkPassword = () => {
		if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/.test(registerData.password)) {
			setErrorMessage("Password does not meet the requirements");
			return false;
		} else return true;
	};

	const checkPasswordsMAtching = () => {
		if (registerData.password !== registerData.confirmPassword) {
			setErrorMessage("Passwords do not match");
			return false;
		} else return true;
	};

	const handleRegister = async () => {
		let isValidRegister =
			!checkForEmptyFields() && checkUsername() && checkPassword() && checkPasswordsMAtching();

		if (isValidRegister) {
			setErrorMessage(null);
			setIsLoading(true);

			await AuthService.register(registerData)
				.then((responseFromServer) => {
					if (responseFromServer) {
						setHasRegistered(true);
						setErrorMessage(null);
					} else setErrorMessage("Error during register, please try again.");
				})
				.catch((error) => {
					error.message === "Duplicate Username"
						? setErrorMessage("The username is already taken. Please use a different one.")
						: setErrorMessage("Error during register, please try again.");
					console.error(error.message);
				})
				.then(() => setIsLoading(false));
		}
	};

	return !hasRegistered ? (
		<>
			<form action="POST" className="form">
				<h1 className="form__title">Register</h1>
				<div className="form__group">
					<label htmlFor="firstName" className="input__label">
						First Name
					</label>
					<input
						id="firstName"
						type="text"
						className="input"
						placeholder="Enter your first name"
						onChange={(e) => setRegisterData({ ...registerData, firstName: e.currentTarget.value })}
					/>
				</div>

				<div className="form__group">
					<label htmlFor="lastName" className="input__label">
						Last Name
					</label>
					<input
						id="lastName"
						type="text"
						className="input"
						placeholder="Enter your last name"
						onChange={(e) => setRegisterData({ ...registerData, lastName: e.currentTarget.value })}
					/>
				</div>

				<div className="form__group">
					<label htmlFor="username" className="input__label">
						Username
					</label>
					<input
						id="username"
						type="text"
						className="input"
						placeholder="Enter a valid email address"
						onChange={(e) => setRegisterData({ ...registerData, username: e.currentTarget.value })}
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
						onChange={(e) => setRegisterData({ ...registerData, password: e.currentTarget.value })}
					/>
				</div>

				<div className="form__group">
					<label htmlFor="confirmPassword" className="input__label">
						Confirm Password
					</label>
					<input
						id="confirmPassword"
						type="password"
						className="input"
						placeholder="Confirm your password"
						onChange={(e) =>
							setRegisterData({ ...registerData, confirmPassword: e.currentTarget.value })
						}
					/>
				</div>

				<div className="form__actions">
					<Button onButtonClick={() => handleRegister()} text="Register" loading={isLoading} />
				</div>

				<p className="register__link" onClick={() => navigate(ROUTES.LOGIN)}>
					Already have an account? Click here to log in
				</p>

				{errorMessage && <p className="error__message">{errorMessage}</p>}
			</form>
		</>
	) : (
		<p className="register__link" onClick={() => navigate(ROUTES.LOGIN)}>
			You have successfully registered. Click here to log in
		</p>
	);
};

export default RegisterForm;
