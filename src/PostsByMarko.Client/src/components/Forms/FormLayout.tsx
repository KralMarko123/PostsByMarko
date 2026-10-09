import type { ReactNode, FormEvent } from "react";
import { useRef } from "react";

interface FormLayoutProps {
  title: string;
  description: string;
  children: ReactNode;
  className?: string;
  onSubmit: () => void | Promise<void>;
}

export const FormLayout = ({
  title,
  description,
  children,
  className = "",
  onSubmit,
}: FormLayoutProps) => {
  const submitting = useRef(false);
  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (submitting.current) return;
    submitting.current = true;
    try {
      await onSubmit();
    } finally {
      submitting.current = false;
    }
  };
  return (
    <form
      noValidate
      onSubmit={submit}
      className={`form flex flex-col gap-5 p-6 sm:p-8 ${className}`}
    >
      <div>
        <h1 className="form-title font-display text-3xl font-medium tracking-tight">{title}</h1>
        <p className="form-desc mt-2 text-muted">{description}</p>
      </div>
      {children}
    </form>
  );
};
