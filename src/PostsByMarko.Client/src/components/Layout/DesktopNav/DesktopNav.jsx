import { React, useContext } from "react";
import { useAuth } from "../../../custom/useAuth";
import { AiOutlineArrowDown } from "react-icons/ai";
import AppContext from "../../../context/AppContext";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import "./DesktopNav.css";

const DesktopNav = () => {
  const { user, logout, isAdmin } = useAuth();
  const appContext = useContext(AppContext);
  const navigate = useNavigate();

  return (
    <>
      <nav className="nav">
        <ul className="nav__content">
          <p className="nav__username">
            Hello{" "}
            <span>
              {user.firstName} {user.lastName}
            </span>
          </p>
          <span className="nav__separator"></span>
          <div className="nav__actions">
            <p>Menu</p>
            <AiOutlineArrowDown />
            <ul className="actions__items">
              <li
                className="action__item"
                onClick={() =>
                  appContext.dispatch({
                    type: "SHOW_MODAL",
                    modal: "createPost",
                  })
                }
              >
                Create Post
              </li>
              {isAdmin && (
                <li
                  className="action__item"
                  onClick={() => navigate(ROUTES.ADMIN)}
                >
                  Dashboard
                </li>
              )}
              <li
                className="action__item"
                onClick={() => navigate(ROUTES.CHAT)}
              >
                Chat
              </li>
              <li className="action__item" onClick={() => logout()}>
                Logout
              </li>
            </ul>
          </div>
        </ul>
      </nav>
    </>
  );
};

export default DesktopNav;
