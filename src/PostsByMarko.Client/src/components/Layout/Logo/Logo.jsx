import React from "react";
import { useNavigate } from "react-router";
import logo from "../../../assets/images/POSM_icon.png";
import { ROUTES } from "../../../constants/routes";
import "./Logo.css";

const Logo = () => {
  const navigate = useNavigate();

  return (
    <img
      src={logo}
      className="logo"
      alt="posm-logo"
      onClick={() => navigate(ROUTES.HOME)}
    />
  );
};

export default Logo;
