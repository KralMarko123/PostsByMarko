import DOMPurify from "dompurify";
import moment from "moment";
import { DateFunctions } from "./dateFunctions";

export const HelperFunctions = {
  noEmptyFields(data) {
    return Object.values(data).every(
      (field) => field.length > 0 || typeof field === "object"
    );
  },

  isValidPassword(password) {
    if (!/^.{6,}$/.test(password))
      return "Password should be at least six characters long";
    if (!/(?=.*[a-z])/.test(password))
      return "Password should contain one lowercase letter";
    if (!/(?=.*[A-Z])/.test(password))
      return "Password should contain one uppercase letter";
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

  sortPostsByCreatedDate(posts) {
    posts.length > 0
      ? posts.sort((p1, p2) => {
          const date1 = Date.parse(p1.createdDate);
          const date2 = Date.parse(p2.createdDate);

          return date1 > date2 ? -1 : 1;
        })
      : null;
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

  getPurifiedHtml(html) {
    return { __html: DOMPurify.sanitize(html) };
  },

  groupMessagesByDay(messages) {
    const messagesByDay = {};

    messages.forEach((m) => {
      const date = moment(m.createdDate);
      const dayKey = date.startOf("day").format("YYYY-MM-DD"); // Format to get a consistent key for each day

      if (!messagesByDay[dayKey]) {
        messagesByDay[dayKey] = [];
      }
      messagesByDay[dayKey].push(m);
    });

    return messagesByDay;
  },

  getMessageTimeLabelAccordingToToday(messageDate) {
    const now = moment();
    let date = moment(messageDate);
    let format = date.isSame(now, "day") ? "h:mm A" : "MMM Do YYYY, h:mm A";

    return DateFunctions.getLocalDateInFormat(date, format);
  },
};
