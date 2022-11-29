export const HelperFunctions = {
	getDateAsReadablestring(date) {
		const readableDate = new Date(Date.parse(date)).toLocaleDateString("en-UK");
		if (readableDate !== "Invalid Date") return readableDate;
		else return "what most likely was a Friday";
	},
};
