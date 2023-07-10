import { React, useEffect } from "react";
import { CSSTransition } from "react-transition-group";
import ReactDOM from "react-dom";
import "./Modal.css";

const Modal = (props) => {
	// in milliseconds
	// should you change this varialbe be sure to change it in the CSS aswell
	const modalTransitionDuration = 300;

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
			timeout={{ enter: 0, exit: modalTransitionDuration }}
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
