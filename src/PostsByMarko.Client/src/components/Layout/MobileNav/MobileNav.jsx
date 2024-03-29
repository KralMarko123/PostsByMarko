import React, { useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import AppContext from "../../../context/AppContext";
import "./MobileNav.css";

const MobileNav = () => {
	const appContext = useContext(AppContext);
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
						{user.firstName} {user.lastName}
					</span>
				</p>
				<div className="mobile__actions">
					<li
						className="actions__item"
						onClick={() => {
							appContext.dispatch({ type: "SHOW_MODAL", modal: "createPost" });
							setIsExpanded(false);
						}}
					>
						Create Post
					</li>
					<li className="actions__item" onClick={() => logout()}>
						Logout
					</li>
				</div>
			</ul>
		</nav>
	);
};

export default MobileNav;
