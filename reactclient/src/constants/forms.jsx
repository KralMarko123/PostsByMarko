import { ICONS } from "./misc";

export const FORMS = {
	registerForm: {
		action: "POST",
		formTitle: "REGISTER",
		formGroups: [
			{
				id: "firstName",
				label: "First Name",
				type: "text",
				placeholder: "First name",
				icon: ICONS.USER_INFO_ICON(),
			},

			{
				id: "lastName",
				label: "Last Name",
				type: "text",
				placeholder: "Last name",
				icon: ICONS.USER_INFO_ICON(),
			},

			{
				id: "username",
				label: "Username",
				type: "text",
				placeholder: "Valid email address",
				requirements: ["Be a valid email address"],
				icon: ICONS.EMAIL_ICON(),
			},

			{
				id: "password",
				label: "Password",
				type: "password",
				placeholder: "Password",
				icon: ICONS.PASSWORD_ICON(),
			},

			{
				id: "confirmPassword",
				label: "Confirm Password",
				type: "password",
				placeholder: "Confirm password",
				icon: ICONS.PASSWORD_ICON(),
			},
		],
	},

	loginForm: {
		action: "POST",
		formTitle: "LOG IN",
		formGroups: [
			{
				id: "username",
				label: "Username",
				type: "text",
				placeholder: "Username",
				icon: ICONS.USER_ICON(),
			},

			{
				id: "password",
				label: "Password",
				type: "password",
				placeholder: "Password",
				icon: ICONS.PASSWORD_ICON(),
			},
		],
	},

	createPostForm: {
		action: "POST",
		formTitle: "Create A Post",
		formGroups: [
			{
				id: "title",
				label: "Title",
				type: "text",
				placeholder: "Title",
				icon: ICONS.TITLE_ICON(),
			},

			{
				id: "content",
				label: "Content",
				type: "textarea",
				placeholder: "Content",
				icon: ICONS.CONTENT_ICON(),
			},
		],
	},

	updatePostForm: {
		action: "POST",
		formTitle: "Update Post",
		formGroups: [
			{
				id: "title",
				label: "Title",
				type: "text",
				placeholder: "Title",
				icon: ICONS.TITLE_ICON(),
			},

			{
				id: "content",
				label: "Content",
				type: "textarea",
				placeholder: "Content",
				icon: ICONS.CONTENT_ICON(),
			},
		],
	},
};
