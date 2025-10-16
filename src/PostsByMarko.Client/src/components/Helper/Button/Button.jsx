import React from "react";
import Loader from "../Loader/Loader";
import "./Button.css";

const Button = ({
  onButtonClick,
  text,
  loading,
  additionalClassNames = "",
}) => {
  const onClick = (e) => {
    e.preventDefault();
    onButtonClick();
  };

  return (
    <button
      className={`button${
        additionalClassNames ? ` ${additionalClassNames}` : ""
      }`}
      onClick={(e) => onClick(e)}
    >
      {loading ? <Loader /> : text}
    </button>
  );
};

export default Button;
