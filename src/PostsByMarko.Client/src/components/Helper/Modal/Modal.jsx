import { React, useEffect } from "react";
import { CSSTransition } from "react-transition-group";
import { modalTransitionDurationInMilliseconds } from "../../../constants/misc";
import ReactDOM from "react-dom";
import "./Modal.css";

const Modal = (props) => {
  const onClose = () => {
    props.onClose();
  };

  useEffect(() => {
    const closeOnEscapeKey = (e) => {
      e.key === "Escape" ? onClose() : null;
    };
    document.addEventListener("keydown", closeOnEscapeKey);
    return () => {
      document.removeEventListener("keydown", closeOnEscapeKey);
    };
  }, []);

  return ReactDOM.createPortal(
    <CSSTransition
      in={props.isShown}
      unmountOnExit
      timeout={{ enter: 0, exit: modalTransitionDurationInMilliseconds }}
    >
      <div className="modal" onClick={() => onClose()}>
        <div className="modal-container" onClick={(e) => e.stopPropagation()}>
          {props.children}
        </div>
      </div>
    </CSSTransition>,
    document.getElementById("app")
  );
};

export default Modal;
