import React from "react";
import { useAuth } from "../custom/useAuth";
import Button from "./UI/Button";
import "../styles/components/ProfileNav.css";

const ProfileNav = () => {
	const { user, logout } = useAuth();

	return (
		<div className="profile__nav">
			<span className="profile__name">{`Hello ${user.userDetails.firstName}`}</span>
			<span className="profile__separator"></span>
			<Button onButtonClick={() => logout()} text={"Logout"} />
		</div>
	);
};

export default ProfileNav;
