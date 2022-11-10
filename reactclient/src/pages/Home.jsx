import React, { useContext } from "react";
import { AppContext } from "../App";
import AllPosts from "./AllPosts";

const Home = () => {
	const { globalState, setGlobalState } = useContext(AppContext);
	console.log(globalState);

	return globalState?.token !== null ? (
		<AllPosts />
	) : (
		<div className="home page">
			<div className="container">
				<h1 className="container__title">Logged out</h1>
			</div>
		</div>
	);
};

export default Home;
