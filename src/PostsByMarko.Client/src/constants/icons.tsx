import { FaUserAlt } from "react-icons/fa";
import { FaGithub } from "react-icons/fa6";
import { RiLockPasswordFill } from "react-icons/ri";
import { MdAlternateEmail, MdDeleteForever } from "react-icons/md";
import {
  AiFillFileText,
  AiFillEye,
  AiFillEyeInvisible,
  AiFillPushpin,
} from "react-icons/ai";
import { BiSolidUserDetail, BiSolidPencil } from "react-icons/bi";
import { BsPersonCircle, BsClockFill } from "react-icons/bs";
import { IoMdSend } from "react-icons/io";
import { IconType } from "react-icons";

export const ICONS: Record<string, IconType> = {
  USER_ICON: FaUserAlt,
  USER_CIRCLE_ICON: BsPersonCircle,
  USER_INFO_ICON: BiSolidUserDetail,
  PASSWORD_ICON: RiLockPasswordFill,
  EMAIL_ICON: MdAlternateEmail,
  PIN_ICON: AiFillPushpin,
  CONTENT_ICON: AiFillFileText,
  PENCIL_ICON: BiSolidPencil,
  DELETE_ICON: MdDeleteForever,
  CLOCK_ICON: BsClockFill,
  GITHUB_ICON: FaGithub,
  SEND_ICON: IoMdSend,
  EYE_ICON_INVISIBLE: AiFillEyeInvisible,
  EYE_ICON: AiFillEye,
};
