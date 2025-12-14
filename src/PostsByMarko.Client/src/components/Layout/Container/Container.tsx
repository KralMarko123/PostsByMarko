import React from "react";
import "./Container.css";

interface ContainerProps {
  title?: string | null;
  desc?: string | null;
  children: React.ReactNode;
}

export const Container = (props: ContainerProps) => {
  return (
    <div className="container">
      {props.title && <h1 className="container-title">{props.title}</h1>}
      {props.desc && <p className="container-desc">{props.desc}</p>}
      {props.children}
    </div>
  );
};
