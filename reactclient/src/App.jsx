import React from "react";
import { Routes, Route } from "react-router-dom";
import { ROUTES } from "./constants/routes";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import Home from "./pages/Home";
import Details from "./pages/Details";
import Login from "./pages/Login";
import Register from "./pages/Register";

const App = () => {
	return (
		<Routes>
			<Route
				path={ROUTES.HOME}
				element={
					<ProtectedRoute>
						<Home />
					</ProtectedRoute>
				}
			/>
			<Route
				path={ROUTES.DETAILS}
				element={
					<ProtectedRoute>
						<Details />
					</ProtectedRoute>
				}
			/>
			<Route path={ROUTES.LOGIN} element={<Login />} />
			<Route path={ROUTES.REGISTER} element={<Register />} />
		</Routes>
	);
};

export default App;
