import React from "react";
import "../../styles/components/Loader.css";

const Loader = () => {
	const numberOfDots = 4;

	return (
		<div className="loader">
			{Array(numberOfDots)
				.fill()
				.map((el, i) => (
					<span key={i} className="dot" style={{ animationDelay: `${i * 0.2}s` }}></span>
				))}
		</div>
	);
};

export default Loader;
