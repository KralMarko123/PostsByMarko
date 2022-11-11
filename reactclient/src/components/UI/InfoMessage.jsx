import React from "react";

const InfoMessage = ({ message, shouldAnimate }) => {
	return <p className={`info__message ${!shouldAnimate ? "no__animation" : ""}`}>{message}</p>;
};

export default InfoMessage;
