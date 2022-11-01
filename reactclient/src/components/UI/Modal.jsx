import { React, useEffect } from "react";
import { CSSTransition } from "react-transition-group";
import ReactDOM from "react-dom";
import "../../styles/components/Modal.css";

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
		<CSSTransition in={props.isShown} unmountOnExit timeout={{ enter: 0, exit: 300 }}>
			<div className="modal" onClick={() => onClose()}>
				<div className="modal__container" onClick={(e) => e.stopPropagation()}>
					<h1 className="modal__title">{props.title}</h1>
					{props.children}
					{props.message && (
						<p className={`modal__message ${props.message.type}`}>{props.message.message}</p>
					)}
				</div>
			</div>
		</CSSTransition>,
		document.getElementById("app")
	);
};

export default Modal;
