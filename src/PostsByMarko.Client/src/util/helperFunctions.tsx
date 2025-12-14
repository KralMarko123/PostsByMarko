import moment from "moment";
import { DateFunctions } from "./dateFunctions";
import { Message } from "@typeConfigs/messaging";

export const HelperFunctions = {
  noEmptyFields(data: Record<string, string>) {
    return Object.values(data).every(
      (field) => field.length > 0 || typeof field !== "string"
    );
  },

  isValidPassword(password: string) {
    if (!/^.{6,}$/.test(password))
      return "Password should be at least six characters long";
    if (!/(?=.*[a-z])/.test(password))
      return "Password should contain one lowercase letter";
    if (!/(?=.*[A-Z])/.test(password))
      return "Password should contain one uppercase letter";
    if (!/(?=.*\d)/.test(password)) return "Password should contain one digit";

    return true;
  },

  groupMessagesByDay(messages: Message[]) {
    let messagesByDay: Record<string, Message[]> = {};

    messages.forEach((m) => {
      const date = moment(m.createdAt);
      const dayKey = date.startOf("day").format("YYYY-MM-DD"); // Format to get a consistent key for each day

      if (!messagesByDay[dayKey]) {
        messagesByDay[dayKey] = [];
      }
      messagesByDay[dayKey].push(m);
    });

    return messagesByDay;
  },

  getMessageTimeLabelAccordingToToday(messageDate: string) {
    const now = moment();
    let date = moment(messageDate);
    let format = date.isSame(now, "day") ? "h:mm A" : "MMM Do YYYY, h:mm A";

    return DateFunctions.getLocalDateInFormat(date, format);
  },
};
