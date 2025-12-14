import "./Loader.css";

export const Loader = () => {
  const dots = [...Array(5).keys()];

  return (
    <div className="loader">
      {dots.map((el, i) => (
        <div key={i} className="dot" style={{ animationDelay: `${i * 0.1}s` }}></div>
      ))}
    </div>
  );
};
