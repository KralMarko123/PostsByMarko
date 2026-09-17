import { useEffect, useRef } from "react";
import { CSSTransition } from "react-transition-group";
import { modalTransitionDurationInMilliseconds } from "../../../constants/misc";
import ReactDOM from "react-dom";
import "./Modal.css";

interface ModalProps {
  onClose: () => void;
  children: React.ReactNode;
  isShown: boolean;
}

export const Modal = ({ onClose, children, isShown }: ModalProps) => {
  const nodeRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const closeOnEscapeKey = (event: KeyboardEvent) => {
      if (event.key === "Escape") onClose();
    };

    document.addEventListener("keydown", closeOnEscapeKey);

    return () => {
      document.removeEventListener("keydown", closeOnEscapeKey);
    };
  }, [onClose]);

  return ReactDOM.createPortal(
    <CSSTransition
      in={isShown}
      nodeRef={nodeRef}
      unmountOnExit
      timeout={{ enter: 0, exit: modalTransitionDurationInMilliseconds }}
    >
      <div ref={nodeRef} className="modal" onClick={onClose}>
        <div className="modal-container" onClick={(e) => e.stopPropagation()}>
          {children}
        </div>
      </div>
    </CSSTransition>,

    document.getElementById("app")!
  );
};
