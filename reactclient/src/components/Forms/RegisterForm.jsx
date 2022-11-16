import React, { useState } from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";
import { FORMS } from "../../constants/forms";
import Button from "../Helper/Button";
import AuthService from "../../api/AuthService";

const RegisterForm = () => {
	const registerForm = FORMS.registerForm;
	const [registerData, setRegisterData] = useState({
		firstName: "",
		lastName: "",
		username: "",
		password: "",
		confirmPassword: "",
	});
	const [isLoading, setIsLoading] = useState(false);
	const [errors, setErrors] = useState({
		title: "",
		messages: [],
	});
	const [isRegistered, setIsRegistered] = useState(false);
	const navigate = useNavigate();

	const checkForEmptyFields = () => {
		let hasEmptyFields = false;

		if (registerData.firstName === "") {
			hasEmptyFields = true;
			setErrors({ title: "First Name can't be empty", messages: [] });
		}

		if (registerData.lastName === "") {
			hasEmptyFields = true;
			setErrors({ title: "Last Name can't be empty", messages: [] });
		}

		if (registerData.username === "") {
			hasEmptyFields = true;
			setErrors({ title: "Username can't be empty", messages: [] });
		}

		if (registerData.password === "") {
			hasEmptyFields = true;
			setErrors({ title: "Password can't be empty", messages: [] });
		}

		if (registerData.confirmPassword === "") {
			hasEmptyFields = true;
			setErrors({ title: "Confirm Password can't be empty", messages: [] });
		}

		return hasEmptyFields;
	};

	const checkUsername = () => {
		if (!/^\S+@\S+\.\S+$/.test(registerData.username)) {
			setErrors({
				title: "Invalid Username",
				messages: ["Username should be a valid email address"],
			});
			return false;
		} else return true;
	};

	const checkPassword = () => {
		let passwordIsValid = true;
		let messages = [];

		if (!/^.{6,}$/.test(registerData.password))
			messages.push("Should be at least six characters long");

		if (!/(?=.*[a-z])/.test(registerData.password)) messages.push("Have one lowercase letter");

		if (!/(?=.*[A-Z])/.test(registerData.password)) messages.push("Have one uppercase letter");

		if (!/(?=.*\d)/.test(registerData.password)) messages.push("Have one digit");

		if (messages.length > 0) {
			passwordIsValid = false;
			setErrors({ title: "Password does not meet the requirements", messages: messages });
		}

		return passwordIsValid;
	};

	const checkPasswordsAreMatching = () => {
		if (registerData.password !== registerData.confirmPassword) {
			setErrors({ title: "Passwords do not match", messages: [] });
			return false;
		} else return true;
	};

	const handleRegister = async () => {
		let isValidRegister =
			!checkForEmptyFields() && checkUsername() && checkPassword() && checkPasswordsAreMatching();

		if (isValidRegister) {
			setErrors(null);
			setIsLoading(true);

			await AuthService.register(registerData)
				.then((responseFromServer) => {
					if (responseFromServer) {
						setIsRegistered(true);
					} else setErrors({ title: "Error during register, please try again", messages: [] });
				})
				.catch((error) => {
					error.message === "Duplicate Username"
						? setErrors({
								title: "The username is already taken. Please use a different one.",
								messages: [],
						  })
						: setErrors({ title: "Error during register, please try again", messages: [] });
					console.error(error.message);
				})
				.then(() => setIsLoading(false));
		}
	};

	return !isRegistered ? (
		<>
			<form action={registerForm.action} className="form">
				<h1 className="form__title">{registerForm.formTitle}</h1>
				{registerForm.formGroups.map((group) => (
					<div key={group.id} className="form__group">
						<label htmlFor={group.id} className="input__label">
							{group.label}
						</label>
						<input
							id={group.id}
							type={group.type}
							className="input"
							placeholder={group.placeholder}
							onChange={(e) =>
								setRegisterData({ ...registerData, [`${group.id}`]: e.currentTarget.value })
							}
						/>
					</div>
				))}

				<div className="form__actions">
					<Button onButtonClick={() => handleRegister()} text="Register" loading={isLoading} />
				</div>

				<p className="link" onClick={() => navigate(ROUTES.LOGIN)}>
					Already have an account? Click here to log in
				</p>

				{errors && (
					<>
						<p className="error error__message">{errors.title}</p>
						{errors.messages.length > 0 &&
							errors.messages.map((m) => (
								<p key={m} className="error__message">
									{m}
								</p>
							))}
					</>
				)}
			</form>
		</>
	) : (
		<p className="link" onClick={() => navigate(ROUTES.LOGIN)}>
			You have successfully registered. Click here to log in
		</p>
	);
};

export default RegisterForm;
