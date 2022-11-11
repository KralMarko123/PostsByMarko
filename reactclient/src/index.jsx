import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";
import "./styles/reset.css";
import "./styles/general.css";
import { AuthProvider } from "./custom/useAuth.jsx";
import { BrowserRouter } from "react-router-dom";

const container = document.getElementById("app");
const root = createRoot(container); // createRoot(container!) if you use TypeScript
root.render(
	<BrowserRouter>
		<AuthProvider>
			<App />
		</AuthProvider>
	</BrowserRouter>
);
