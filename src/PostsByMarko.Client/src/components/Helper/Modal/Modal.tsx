import { useEffect, useRef } from "react";
import { createPortal } from "react-dom";

interface ModalProps {
  onClose: () => void;
  children: React.ReactNode;
  isShown: boolean;
  title: string;
}

export const Modal = ({ onClose, children, isShown, title }: ModalProps) => {
  const dialogRef = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = dialogRef.current!;
    if (isShown && !dialog.open) {
      dialog.showModal();
      dialog.querySelector<HTMLElement>("input, textarea, button")?.focus();
    } else if (!isShown && dialog.open) {
      dialog.close();
    }
    return () => {
      if (dialog.open) dialog.close();
    };
  }, [isShown]);

  return createPortal(
    <dialog
      ref={dialogRef}
      aria-label={title}
      className="modal fixed inset-0 m-auto max-h-[85dvh] w-[calc(100%-2rem)] max-w-2xl overflow-y-auto rounded-2xl border border-line bg-surface p-0 text-ink shadow-card backdrop:bg-black/70"
      onCancel={(event) => {
        event.preventDefault();
        onClose();
      }}
      onClick={(event) => {
        if (event.target !== event.currentTarget) return;
        const bounds = event.currentTarget.getBoundingClientRect();
        if (
          event.clientX < bounds.left ||
          event.clientX > bounds.right ||
          event.clientY < bounds.top ||
          event.clientY > bounds.bottom
        )
          onClose();
      }}
    >
      {isShown && children}
    </dialog>,
    document.body,
  );
};
