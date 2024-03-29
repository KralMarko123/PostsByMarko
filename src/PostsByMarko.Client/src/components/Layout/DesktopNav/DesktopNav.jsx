import { React, useContext } from "react";
import { useAuth } from "../../../custom/useAuth";
import { AiOutlineArrowDown } from "react-icons/ai";
import AppContext from "../../../context/AppContext";
import "./DesktopNav.css";

const DesktopNav = () => {
	const { user, logout } = useAuth();
	const appContext = useContext(AppContext);

	return (
		<>
			<nav className="nav">
				<ul className="nav__content">
					<p className="nav__username">
						Hello{" "}
						<span>
							{user.firstName} {user.lastName}
						</span>
					</p>
					<span className="nav__separator"></span>
					<div className="nav__actions">
						<p>Menu</p>
						<AiOutlineArrowDown />
						<ul className="actions__items">
							<li
								className="action__item"
								onClick={() => appContext.dispatch({ type: "SHOW_MODAL", modal: "createPost" })}
							>
								Create Post
							</li>
							<li className="action__item" onClick={() => logout()}>
								Logout
							</li>
						</ul>
					</div>
				</ul>
			</nav>
		</>
	);
};

export default DesktopNav;
