import { FaUserAlt } from "react-icons/fa";
import { RiLockPasswordFill } from "react-icons/ri";
import { MdAlternateEmail, MdDeleteForever } from "react-icons/md";
import { AiFillFileText, AiFillEye, AiFillEyeInvisible, AiFillPushpin } from "react-icons/ai";
import { BiSolidUserDetail, BiSolidPencil } from "react-icons/bi";
import { BsPersonCircle, BsClockFill } from "react-icons/bs";
//in milliseconds
export const modalTransitionDuration = 300;

export const ICONS = {
	USER_ICON() {
		return <FaUserAlt />;
	},
	USER_CIRCLE_ICON() {
		return <BsPersonCircle />;
	},
	USER_INFO_ICON() {
		return <BiSolidUserDetail />;
	},
	PASSWORD_ICON() {
		return <RiLockPasswordFill />;
	},
	EMAIL_ICON() {
		return <MdAlternateEmail />;
	},
	PIN_ICON() {
		return <AiFillPushpin />;
	},
	CONTENT_ICON() {
		return <AiFillFileText />;
	},
	EYE_ICON(hidden) {
		return !hidden ? <AiFillEye /> : <AiFillEyeInvisible />;
	},
	PENCIL_ICON() {
		return <BiSolidPencil />;
	},
	DELETE_ICON() {
		return <MdDeleteForever />;
	},
	CLOCK_ICON() {
		return <BsClockFill />;
	},
};
