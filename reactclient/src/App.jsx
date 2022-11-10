import React, { createContext, useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { ROUTES } from "./constants/routes";
import Details from "./pages/Details";
import Home from "./pages/Home";
import AllPosts from "./pages/AllPosts";

export const AppContext = createContext({
	token: null,
	username: null,
});

const App = () => {
	const [globalState, setGlobalState] = useState({
		token: null,
		username: null,
	});

	useEffect(() => {
		const token = JSON.parse(window.sessionStorage.getItem("token"));
		if (token) setAppState({ ...globalState, token: token });
	}, []);

	return (
		<Router>
			<Routes>
				<Route
					path={ROUTES.ALL_POSTS}
					element={
						<AppContext.Provider value={{ globalState, setGlobalState }}>
							<AllPosts />
						</AppContext.Provider>
					}
				/>
				<Route
					path={ROUTES.HOME}
					element={
						<AppContext.Provider value={{ globalState, setGlobalState }}>
							<Home />
						</AppContext.Provider>
					}
				/>
				<Route path={ROUTES.DETAILS} element={<Details />} />
			</Routes>
		</Router>
	);
};

export default App;
