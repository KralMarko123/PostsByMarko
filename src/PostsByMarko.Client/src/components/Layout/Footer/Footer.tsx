import { githubLink } from "../../../constants/misc";
import { ICONS } from "../../../constants/icons";

export const Footer = () => {
  return (
    <footer className="footer mx-auto flex w-full max-w-6xl flex-wrap items-center justify-between gap-4 px-5 py-6 text-sm text-muted sm:px-8">
      <div className="copyright">© PostsByMarko | All rights reserved.</div>
      <div className="links">
        <a
          className="footer-link inline-flex rounded-md p-2 text-xl hover:text-mint"
          href={githubLink}
          target="_blank"
          rel="noopener noreferrer"
          aria-label="View project on GitHub"
        >
          {ICONS.GITHUB_ICON({})}
        </a>
      </div>
    </footer>
  );
};
