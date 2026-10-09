import { useRef, useState } from "react";
import { Loader } from "../Loader/Loader";

interface ButtonProps {
  onButtonClick?: () => void | Promise<void>;
  type?: "button" | "submit";
  disabled?: boolean;
  text: string;
  loading?: boolean | null;
  additionalClassNames?: string | null;
  variant?: "primary" | "secondary" | "danger";
}

export const Button = (props: ButtonProps) => {
  const variants = {
    primary: "bg-brand text-white hover:bg-brand-hover",
    secondary: "border border-line bg-elevated text-ink hover:bg-line",
    danger: "bg-danger-surface text-ink hover:bg-danger-surface/80",
  };
  const inFlight = useRef(false);
  const [pending, setPending] = useState(false);
  const onClick = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();

    if (inFlight.current || props.loading || props.disabled) return;
    inFlight.current = true;
    setPending(true);
    try {
      await props.onButtonClick?.();
    } finally {
      inFlight.current = false;
      setPending(false);
    }
  };

  return (
    <button
      type={props.type ?? "button"}
      disabled={Boolean(props.disabled || props.loading || pending)}
      aria-busy={Boolean(props.loading || pending)}
      className={`button inline-flex min-h-11 items-center justify-center gap-2 rounded-lg px-5 py-2.5 text-base font-semibold transition-colors disabled:cursor-not-allowed disabled:opacity-50 ${variants[props.variant ?? "primary"]} ${props.additionalClassNames ?? ""}`}
      onClick={props.type === "submit" ? undefined : (e) => onClick(e)}
    >
      {(props.loading || pending) && <Loader />}
      <span>{props.text}</span>
    </button>
  );
};
