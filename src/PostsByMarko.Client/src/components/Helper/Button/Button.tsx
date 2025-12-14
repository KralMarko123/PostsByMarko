import { Loader } from "../Loader/Loader";
import "./Button.css";

interface ButtonProps {
  onButtonClick: () => void;
  text: string;
  loading?: boolean | null;
  additionalClassNames?: string | null;
}

export const Button = (props: ButtonProps) => {
  const onClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();

    props.onButtonClick();
  };

  return (
    <button
      className={`button${
        props.additionalClassNames ? ` ${props.additionalClassNames}` : ""
      }`}
      onClick={(e) => onClick(e)}
    >
      {props.loading ? <Loader /> : props.text}
    </button>
  );
};
