import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { ROUTES } from "./constants/routes";
import Details from "./pages/Details";
import Home from "./pages/Home";

const App = () => {
	return (
		<Router>
			<Routes>
				<Route path={ROUTES.HOME} element={<Home />} />
				<Route path={ROUTES.DETAILS} element={<Details />} />
			</Routes>
		</Router>
	);
};

export default App;
