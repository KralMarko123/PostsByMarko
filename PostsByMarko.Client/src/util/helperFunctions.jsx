export const HelperFunctions = {
	getDateAsReadableText(date) {
		const readableDate = new Date(Date.parse(date)).toLocaleDateString("en-UK");
		if (readableDate !== "Invalid Date") return readableDate;
		else return "what most likely was a Friday";
	},

	noEmptyFields(data) {
		return Object.values(data).every((field) => field.length > 0);
	},

	isValidPassword(password) {
		if (!/^.{6,}$/.test(password)) return "Password should be at least six characters long";
		if (!/(?=.*[a-z])/.test(password)) return "Password should contain one lowercase letter";
		if (!/(?=.*[A-Z])/.test(password)) return "Password should contain one uppercase letter";
		if (!/(?=.*\d)/.test(password)) return "Password should contain one digit";

		return true;
	},

	getErrorMessageForFailingResponse(responseMessage) {
		switch (responseMessage) {
			case "NO ACCOUNT":
				return "No account found, please check your credentials and try again";
			case "NOT CONFIRMED":
				return "Please check your email and confirm your account before logging in";
			case "INVALID PASSWORD":
				return "Invalid password for the given account";
			case "DUPLICATEUSERNAME":
				return "Username has already been taken, please try again with a different one";
			default:
				return "Error during submission, please try again later";
		}
	},

	applyFilters(posts, filters, userId) {
		let filteredPosts = posts;

		Object.entries(filters).forEach((filter) => {
			const [name, isApplied] = filter;

			switch (name) {
				case "showOnlyMyPosts":
					filteredPosts = isApplied ? this.showOnlyMyPosts(filteredPosts, userId) : filteredPosts;
					break;
				case "showHiddenPosts":
					filteredPosts = isApplied ? filteredPosts : this.filterHiddenPosts(posts, false);
					break;

				default:
					break;
			}
		});

		this.sortPostsByLastUpdatedDate(filteredPosts);
		return filteredPosts;
	},

	sortPostsByLastUpdatedDate(posts) {
		posts.length > 0
			? posts.sort((p1, p2) => {
					const date1 = Date.parse(p1.lastUpdatedDate);
					const date2 = Date.parse(p2.lastUpdatedDate);

					return date1 > date2 ? -1 : 1;
			  })
			: null;
	},

	showOnlyMyPosts(posts, userId) {
		return posts.length > 0 ? posts.filter((p) => p.userId === userId) : posts;
	},

	filterHiddenPosts(posts, hidden) {
		return posts.length > 0 ? posts.filter((p) => p.isHidden === hidden) : posts;
	},
};
