import { React, useEffect } from "react";
import { CSSTransition } from "react-transition-group";
import { modalTransitionDuration } from "../../constants/misc";
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
		<CSSTransition
			in={props.isShown}
			timeout={{ enter: 0, exit: modalTransitionDuration }}
			unmountOnExit
		>
			<div
				className="modal"
				onClick={() => onClose()}
				style={{ transitionDuration: `${modalTransitionDuration / 1000}s` }}
			>
				<div
					className="modal__container"
					onClick={(e) => e.stopPropagation()}
					style={{ transitionDuration: `${modalTransitionDuration / 1000}s` }}
				>
					<h1 className="modal__title">{props.title}</h1>
					{props.children}
				</div>
			</div>
		</CSSTransition>,
		document.getElementById("app")
	);
};

export default Modal;
