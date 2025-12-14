import { createRoot } from "react-dom/client";
import { AuthProvider } from "./custom/useAuth.js";
import { BrowserRouter } from "react-router-dom";
import App from "./App.js";
import "./styles/general.css";
import "./styles/reset.css";

const container: HTMLElement | null = document.getElementById("app");
const root = createRoot(container!); // createRoot(container!) if you use TypeScript

root.render(
  <BrowserRouter>
    <AuthProvider>
      <App />
    </AuthProvider>
  </BrowserRouter>
);
