import moment from "moment";

export const DateFunctions = {
  getDateAsReadableText(date) {
    const readableDate = new Date(Date.parse(date)).toLocaleDateString("en-UK");
    if (readableDate !== "Invalid Date") return readableDate;
    else return "what most likely was a Friday";
  },

  getDateAsLocalDate(date) {
    return moment(moment.utc(date)).local();
  },

  getReadableDateTime(dateTime) {
    const dateLocal = this.getDateAsLocalDate(dateTime);
    const dayAndMonth = moment(dateLocal).format("Do MMMM");
    const hoursAndMinutes = moment(dateLocal).format("HH:mm");

    return `${dayAndMonth} at ${hoursAndMinutes}`;
  },

  getLocalDateInFormat(date, format = "DD MMMM YYYY") {
    const dateLocal = this.getDateAsLocalDate(date);

    return moment(dateLocal).format(format);
  },

  getCurrentMonthDayNumber() {
    return moment().daysInMonth();
  },

  getDayOfMonthFromDate(date) {
    return moment(date).format("DD");
  },

  getThisMonthsDates() {
    const dates = [];
    let current = moment().startOf("month");
    const end = moment().endOf("month");

    while (current.isSameOrBefore(end, "day")) {
      dates.push(moment(current.clone()).format("D/M/YYYY")); // Clone to avoid modifying the original date in the loop
      current.add(1, "day");
    }

    return dates;
  },

  getDateTimeInFormat(dateTime, format) {
    return moment(dateTime).format(format);
  },

  getThisMonthAsText() {
    return moment().format("MMMM");
  },
};
