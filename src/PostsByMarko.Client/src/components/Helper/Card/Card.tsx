import "./Card.css";

interface CardProps {
  children: React.ReactNode;
}

export const Card = (props: CardProps) => {
  return <div className={"card"}>{props.children}</div>;
};
