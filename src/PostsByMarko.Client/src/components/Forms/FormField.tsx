import { useId } from "react";
import type { ReactNode } from "react";

interface FormFieldProps {
  name: string;
  label: string;
  type?: string;
  placeholder?: string;
  icon?: ReactNode;
  autoComplete?: string;
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
  maxLength?: number;
}

export const FormField = ({
  name,
  label,
  type = "text",
  placeholder,
  icon,
  autoComplete,
  value,
  onChange,
  disabled,
  maxLength,
}: FormFieldProps) => {
  const fieldId = useId();
  const inputClass = `input min-h-12 w-full rounded-lg border border-line bg-page px-4 py-3 text-base text-ink placeholder:text-muted/70 focus:border-mint focus:outline-none focus:ring-2 focus:ring-mint/20 disabled:opacity-50 ${icon ? "pl-11" : ""}`;
  return (
    <div className="form-group flex flex-col gap-2">
      <label htmlFor={fieldId} className="text-sm font-semibold text-muted">
        {label}
      </label>
      <div className="relative">
        {type === "textarea" ? (
          <textarea
            id={fieldId}
            name={name}
            className={`${inputClass} input-text min-h-44 resize-y`}
            rows={6}
            value={value}
            placeholder={placeholder}
            disabled={disabled}
            maxLength={maxLength}
            onChange={(event) => onChange(event.currentTarget.value)}
          />
        ) : (
          <input
            id={fieldId}
            name={name}
            type={type}
            className={inputClass}
            value={value}
            placeholder={placeholder}
            autoComplete={autoComplete}
            disabled={disabled}
            maxLength={maxLength}
            onChange={(event) => onChange(event.currentTarget.value)}
          />
        )}
        {icon && (
          <span
            aria-hidden="true"
            className="pointer-events-none absolute top-4 left-4 text-lg text-muted"
          >
            {icon}
          </span>
        )}
      </div>
    </div>
  );
};
