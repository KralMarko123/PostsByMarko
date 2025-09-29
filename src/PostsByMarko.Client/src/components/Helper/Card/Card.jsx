import React from "react";
import "./Card.css";

const Card = (props) => {
  return (
    <div className={`card${props.buttonRadius ? " button-radius" : ""}`}>
      {props.children}
    </div>
  );
};

export default Card;
