import React from "react";
import { githubLink } from "../../../constants/misc";
import { ICONS } from "../../../constants/icons";
import "./Footer.css";

const Footer = () => {
  return (
    <div className="footer">
      <div className="links">
        <span
          className="footer-link"
          onClick={() => window.open(githubLink, "_blank")}
        >
          {ICONS.GITHUB_ICON()}
        </span>
      </div>
      <div className="copyright">© PostsByMarko | All rights reserved.</div>
    </div>
  );
};

export default Footer;
