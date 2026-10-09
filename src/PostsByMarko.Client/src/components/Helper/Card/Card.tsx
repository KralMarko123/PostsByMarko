interface CardProps {
  children: React.ReactNode;
  className?: string;
}

export const Card = (props: CardProps) => {
  return (
    <div
      className={`card rounded-2xl border border-line/70 bg-surface shadow-card ${props.className ?? ""}`}
    >
      {props.children}
    </div>
  );
};
