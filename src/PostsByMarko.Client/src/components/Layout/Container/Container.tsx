import React from "react";

interface ContainerProps {
  title?: string | null;
  desc?: string | null;
  children: React.ReactNode;
}

export const Container = (props: ContainerProps) => {
  return (
    <div className="mx-auto w-full max-w-6xl px-5 py-8 sm:px-8 sm:py-12">
      {props.title && (
        <h1 className="container-title font-display text-3xl font-medium tracking-tight sm:text-4xl">
          {props.title}
        </h1>
      )}
      {props.desc && <p className="container-desc mt-2 mb-8 max-w-2xl text-muted">{props.desc}</p>}
      {props.children}
    </div>
  );
};
