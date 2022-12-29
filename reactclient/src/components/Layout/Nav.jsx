import React, { useState, useEffect, useContext } from "react";
import AppContext from "../../context/AppContext";
import CreatePostForm from "../Forms/CreatePostForm";
import DesktopNav from "./DesktopNav";
import MobileNav from "./MobileNav";

const Nav = () => {
	const appContext = useContext(AppContext);
	const [width, setWidth] = useState(window.innerWidth);

	const handleWindowResize = () => {
		setWidth(window.innerWidth);
	};

	useEffect(() => {
		window.addEventListener("resize", handleWindowResize);
		return () => window.removeEventListener("resize", handleWindowResize);
	}, []);
	return (
		<>
			{width <= 1199 ? <MobileNav /> : <DesktopNav />}
			<CreatePostForm />
		</>
	);
};

export default Nav;
