import { ICONS } from "./icons";
import { ReactNode } from "react";

interface FormGroup {
  id: string;
  label?: string | null;
  type: string;
  placeholder: string;
  icon?: ReactNode;
}

export const FORMS: Record<string, Record<string, FormGroup[]>> = {
  REGISTER_FORM: {
    formGroups: [
      {
        id: "firstName",
        label: "First Name",
        type: "text",
        placeholder: "First name",
        icon: <ICONS.USER_INFO_ICON />,
      },

      {
        id: "lastName",
        label: "Last Name",
        type: "text",
        placeholder: "Last name",
        icon: <ICONS.USER_INFO_ICON />,
      },

      {
        id: "email",
        label: "Email",
        type: "text",
        placeholder: "Email",
        icon: <ICONS.USER_ICON />,
      },

      {
        id: "password",
        label: "Password",
        type: "password",
        placeholder: "Password",
        icon: <ICONS.PASSWORD_ICON />,
      },

      {
        id: "confirmPassword",
        label: "Confirm Password",
        type: "password",
        placeholder: "Confirm password",
        icon: <ICONS.PASSWORD_ICON />,
      },
    ],
  },

  LOGIN_FORM: {
    formGroups: [
      {
        id: "email",
        type: "text",
        placeholder: "Email",
        icon: <ICONS.USER_ICON />,
      },

      {
        id: "password",
        type: "password",
        placeholder: "Password",
        icon: <ICONS.PASSWORD_ICON />,
      },
    ],
  },

  CREATE_POST_FORM: {
    formGroups: [
      {
        id: "title",
        label: "Title",
        type: "text",
        placeholder: "Title",
        icon: <ICONS.PIN_ICON />,
      },

      {
        id: "content",
        label: "Content",
        type: "textarea",
        placeholder: "Content",
      },
    ],
  },

  UPDATE_POST_FORM: {
    formGroups: [
      {
        id: "title",
        label: "Title",
        type: "text",
        placeholder: "Title",
        icon: <ICONS.PIN_ICON />,
      },

      {
        id: "content",
        label: "Content",
        type: "textarea",
        placeholder: "Content",
      },
    ],
  },
};
