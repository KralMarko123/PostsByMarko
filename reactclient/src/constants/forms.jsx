export const FORMS = {
	registerForm: {
		action: "POST",
		formTitle: "Register",
		formGroups: [
			{
				id: "firstName",
				label: "First Name",
				type: "text",
				placeholder: "Enter your first name",
			},

			{
				id: "lastName",
				label: "Last Name",
				type: "text",
				placeholder: "Enter your last name",
			},

			{
				id: "username",
				label: "Username",
				type: "text",
				placeholder: "Enter a valid email address",
				requirements: ["Be a valid email address"],
			},

			{
				id: "password",
				label: "Password",
				type: "password",
				placeholder: "Enter your password",
			},

			{
				id: "confirmPassword",
				label: "Confirm Password",
				type: "password",
				placeholder: "Confirm your password",
			},
		],
	},

	loginForm: {
		action: "POST",
		formTitle: "Login",
		formGroups: [
			{
				id: "username",
				label: "Username",
				type: "text",
				placeholder: "Enter your username",
			},

			{
				id: "password",
				label: "Password",
				type: "password",
				placeholder: "Enter your password",
			},
		],
	},
};
