import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";
import "./styles/reset.css";
import "./styles/general.css";

const container = document.getElementById("app");
const root = createRoot(container); // createRoot(container!) if you use TypeScript
root.render(
	<React.StrictMode>
		<App />
	</React.StrictMode>
);
