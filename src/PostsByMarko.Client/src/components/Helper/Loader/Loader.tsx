export const Loader = () => {
  const dots = [...Array(5).keys()];

  return (
    <span className="loader inline-flex items-center gap-1" aria-hidden="true">
      {dots.map((el, i) => (
        <span
          key={i}
          className="size-1 animate-dot-buffer rounded-full bg-current motion-reduce:animate-none"
          style={{ animationDelay: `${i * 0.1}s` }}
        />
      ))}
    </span>
  );
};
