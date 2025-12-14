import { Loader } from "../Loader/Loader";
import "./Button.css";

interface ButtonProps {
  onButtonClick: () => void;
  text: string;
  loading?: boolean | null;
  additionalClassNames?: string | null;
}

export const Button = (props: ButtonProps) => {
  return (
    <button
      className={`button${
        props.additionalClassNames ? ` ${props.additionalClassNames}` : ""
      }`}
      onClick={props.onButtonClick}
    >
      {props.loading ? <Loader /> : props.text}
    </button>
  );
};
