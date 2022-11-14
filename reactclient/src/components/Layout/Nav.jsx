import React, { useState, useEffect } from "react";
import { useAuth } from "../../custom/useAuth";
import DesktopNav from "../DesktopNav";
import MobileNav from "../MobileNav";

const Nav = () => {
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
	return width <= 1199 ? <MobileNav /> : <DesktopNav />;
};

export default Nav;
