import { Link } from "react-router-dom";
import logo from "../../../assets/images/POSM_icon.png";
import { ROUTES } from "../../../constants/routes";

export const Logo = () => {
  return (
    <Link
      to={ROUTES.HOME}
      aria-label="PostsByMarko home"
      className="flex min-w-0 items-center gap-2 rounded-lg sm:gap-3"
    >
      <img src={logo} className="logo size-10 shrink-0 object-contain sm:size-12" alt="" />
      <span className="truncate font-display text-lg tracking-wide sm:text-xl">PostsByMarko</span>
    </Link>
  );
};
