import React, { useState, useEffect } from "react";
import { useAuth } from "../custom/useAuth";
import Button from "./Helper/Button";
import "../styles/components/DesktopNav.css";

const DesktopNav = () => {
	const { user, logout } = useAuth();
	const [width, setWidth] = useState(window.innerWidth);
	const [isExpanded, setIsExpanded] = useState(false);

	const handleWindowResize = () => {
		setWidth(window.innerWidth);
	};

	useEffect(() => {
		window.addEventListener("resize", handleWindowResize);
		return () => window.removeEventListener("resize", handleWindowResize);
	}, []);

	return (
		<nav className="nav">
			<ul className="nav__content">
				<p className="nav__username">
					Hello{" "}
					<span>
						{user.userDetails.firstName} {user.userDetails.lastName}
					</span>
				</p>
				<span className="nav__separator"></span>
				<div className="nav__actions">
					<Button onButtonClick={() => logout()} text={"Logout"} />
				</div>
			</ul>
		</nav>
	);
};

export default DesktopNav;
