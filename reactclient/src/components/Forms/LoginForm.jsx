import React, { useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";
import { FORMS } from "../../constants/forms";
import { HelperFunctions } from "../../util/helperFunctions";
import Button from "../Helper/Button";
import AuthService from "../../api/AuthService";

const LoginForm = () => {
	const loginForm = FORMS.loginForm;
	const [loginData, setLoginData] = useState({
		username: "",
		password: "",
	});
	const [isLoading, setIsLoading] = useState(false);
	const [errors, setErrors] = useState({
		title: "",
		messages: [],
	});
	const { login } = useAuth();
	const navigate = useNavigate();

	const checkForEmptyFields = () => {
		if (!HelperFunctions.checkForEmptyFields(loginData)) {
			setErrors({ title: "Fields can't be empty", messages: [] });
			return true;
		} else return false;
	};

	const handleLogin = async () => {
		let isValidLogin = !checkForEmptyFields();

		if (isValidLogin) {
			setErrors({
				title: "",
				messages: [],
			});
			setIsLoading(true);

			await AuthService.login(loginData)
				.then((response) => {
					if (response.token) login(response);
					else
						setErrors({
							title: HelperFunctions.getErrorMessageForFailingResponse(response),
							messages: [],
						});
				})
				.finally(() => setIsLoading(false));
		}
	};

	return (
		<form action={loginForm.action} className="form">
			<h1 className="form__title">{loginForm.formTitle}</h1>
			{loginForm.formGroups.map((group) => (
				<div key={group.id} className="form__group">
					{group.icon}
					<input
						id={group.id}
						type={group.type}
						className="input"
						placeholder={group.placeholder}
						onChange={(e) => setLoginData({ ...loginData, [`${group.id}`]: e.currentTarget.value })}
					/>
				</div>
			))}

			<div className="form__actions">
				<Button onButtonClick={() => handleLogin()} text="Log In" loading={isLoading} />
			</div>
			<p className="link" onClick={() => navigate(ROUTES.REGISTER)}>
				Haven't registered yet? Click here to create an account
			</p>

			{errors.title && (
				<>
					<p className="error">{errors.title}</p>
					{errors.messages.length > 0 &&
						errors.messages.map((m) => (
							<p key={m} className="error__message">
								{m}
							</p>
						))}
				</>
			)}
		</form>
	);
};

export default LoginForm;
