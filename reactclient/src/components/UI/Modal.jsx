import { React, useEffect } from "react";
import { CSSTransition } from "react-transition-group";
import ReactDOM from "react-dom";
import "../../styles/components/Modal.css";

const Modal = (props) => {
	const onClose = () => {
		props.onClose();
		console.log("closing", props.duration * 100);
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
			timeout={{ enter: 0, exit: props.duration * 1000 }}
			unmountOnExit
		>
			<div
				className="modal"
				onClick={() => onClose()}
				style={{ transitionDuration: `${props.duration}s` }}
			>
				<div
					className="modal__container"
					onClick={(e) => e.stopPropagation()}
					style={{ transitionDuration: `${props.duration}s` }}
				>
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
