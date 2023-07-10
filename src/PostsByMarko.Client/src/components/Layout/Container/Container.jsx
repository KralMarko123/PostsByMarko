import React from "react";
import "./Container.css";

const Container = (props) => {
	return (
		<div className="container">
			{props.title && <h1 className="container-title">{props.title}</h1>}
			{props.desc && <p className="container-desc">{props.desc}</p>}
			{props.children}
		</div>
	);
};

export default Container;
