import React from "react";
import "../../styles/components/Button.css";

const Button = ({ onButtonClick, text }) => {
	const onClick = (e) => {
		e.preventDefault();
		onButtonClick();
	};

	return (
		<button className="button" onClick={(e) => onClick(e)}>
			{text}
		</button>
	);
};

export default Button;
