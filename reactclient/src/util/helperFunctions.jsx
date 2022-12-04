export const HelperFunctions = {
	getDateAsReadableText(date) {
		const readableDate = new Date(Date.parse(date)).toLocaleDateString("en-UK");
		if (readableDate !== "Invalid Date") return readableDate;
		else return "what most likely was a Friday";
	},

	checkForEmptyFields(data) {
		return Object.values(data).every((field) => field.length > 0);
	},

	checkPasswordRequirements(password) {
		let passwordValidator = {
			isValid: true,
			messages: [],
		};

		if (!/^.{6,}$/.test(password))
			passwordValidator.messages.push("Should be at least six characters long");
		if (!/(?=.*[a-z])/.test(password)) passwordValidator.messages.push("Have one lowercase letter");
		if (!/(?=.*[A-Z])/.test(password)) passwordValidator.messages.push("Have one uppercase letter");
		if (!/(?=.*\d)/.test(password)) passwordValidator.messages.push("Have one digit");
		if (passwordValidator.messages.length > 0) passwordValidator.isValid = false;

		return passwordValidator;
	},
};
