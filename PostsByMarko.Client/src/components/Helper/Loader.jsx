import React from "react";
import "../../styles/components/Loader.css";

const Loader = () => {
	const numberOfDots = 5;

	return (
		<div className="loader">
			{Array(numberOfDots)
				.fill()
				.map((el, i) => (
					<div key={i} className="dot" style={{ animationDelay: `${i * 0.1}s` }}></div>
				))}
		</div>
	);
};

export default Loader;
