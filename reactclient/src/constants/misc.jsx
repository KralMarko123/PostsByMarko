import { FaUserAlt } from "react-icons/fa";
import { RiLockPasswordFill } from "react-icons/ri";
import { MdAlternateEmail, MdTitle } from "react-icons/md";
import { GrContactInfo } from "react-icons/gr";
import { AiFillFileText, AiFillEye, AiFillEyeInvisible } from "react-icons/ai";

//in milliseconds
export const modalTransitionDuration = 300;

export const ICONS = {
	USER_ICON() {
		return <FaUserAlt />;
	},
	USER_INFO_ICON() {
		return <GrContactInfo />;
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
};
