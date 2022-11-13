import React from "react";
import { useAuth } from "../custom/useAuth";
import Button from "./UI/Button";
import "../styles/components/Nav.css";

const ProfileNav = () => {
	const { user, logout } = useAuth();

	return (
		<nav className="nav">
			<div className="nav__profile">
				<p className="profile__name">Hello  <span>{user.userDetails.firstName} {user.userDetails.lastName}</span></p>
				<span className="profile__separator"></span>
				<Button onButtonClick={() => logout()} text={"Logout"} />
			</div>
		</nav>
	);
};

export default ProfileNav;
