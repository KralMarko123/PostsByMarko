import { useRef, useState } from "react";
import { Loader } from "../Loader/Loader";
import "./Button.css";

interface ButtonProps {
  onButtonClick: () => void | Promise<void>;
  disabled?: boolean;
  text: string;
  loading?: boolean | null;
  additionalClassNames?: string | null;
}

export const Button = (props: ButtonProps) => {
  const inFlight = useRef(false);
  const [pending, setPending] = useState(false);
  const onClick = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();

    if (inFlight.current || props.loading || props.disabled) return;
    inFlight.current = true;
    setPending(true);
    try {
      await props.onButtonClick();
    } finally {
      inFlight.current = false;
      setPending(false);
    }
  };

  return (
    <button
      type="button"
      disabled={Boolean(props.disabled || props.loading || pending)}
      aria-busy={Boolean(props.loading || pending)}
      className={`button${
        props.additionalClassNames ? ` ${props.additionalClassNames}` : ""
      }`}
      onClick={(e) => onClick(e)}
    >
      {props.loading ? <Loader /> : props.text}
    </button>
  );
};
