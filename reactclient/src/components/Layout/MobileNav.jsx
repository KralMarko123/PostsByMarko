import React, { useState } from "react";
import { useAuth } from "../../custom/useAuth";
import Button from "../Helper/Button";
import "../../styles/components/MobileNav.css";

const MobileNav = () => {
	const { user, logout } = useAuth();
	const [isExpanded, setIsExpanded] = useState(false);

	return (
		<nav className={`mobile__nav ${isExpanded ? "open" : ""}`}>
			<div className="hamburger" onClick={() => setIsExpanded((prev) => !prev)}>
				<span className="hamburger__line"></span>
				<span className="hamburger__line"></span>
				<span className="hamburger__line"></span>
			</div>

			<ul className="nav__content">
				<p className="nav__username">
					Hello{" "}
					<span>
						{user.userProfile.firstName} {user.userProfile.lastName}
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

export default MobileNav;
