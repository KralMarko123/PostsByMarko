import { FaUserAlt } from "react-icons/fa";
import { RiLockPasswordFill } from "react-icons/ri";
import { MdAlternateEmail, MdTitle, MdDeleteForever } from "react-icons/md";
import { AiFillFileText, AiFillEye, AiFillEyeInvisible } from "react-icons/ai";
import { BiSolidUserDetail, BiSolidPencil } from "react-icons/bi";

//in milliseconds
export const modalTransitionDuration = 300;

export const ICONS = {
	USER_ICON() {
		return <FaUserAlt />;
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
	TITLE_ICON() {
		return <MdTitle />;
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
};
