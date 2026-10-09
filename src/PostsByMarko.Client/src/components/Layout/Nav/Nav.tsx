import { useContext, useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { AiOutlineDown } from "react-icons/ai";
import { useAuth } from "../../../custom/useAuth";
import { AppContext } from "../../../context/AppContext";
import { ROUTES } from "../../../constants/routes";
import { CreatePostForm } from "../../Forms/CreatePostForm/CreatePostForm";

const menuItem = "action__item block w-full rounded-lg px-4 py-2.5 text-left hover:bg-elevated";

export const Nav = () => {
  const { user, logout, isAdmin } = useAuth();
  const { dispatch } = useContext(AppContext);
  const [isExpanded, setIsExpanded] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    if (!isExpanded) return;
    const closeOutside = (event: PointerEvent) => {
      if (!menuRef.current?.contains(event.target as Node)) setIsExpanded(false);
    };
    document.addEventListener("pointerdown", closeOutside);
    return () => document.removeEventListener("pointerdown", closeOutside);
  }, [isExpanded]);

  return (
    <>
      <nav aria-label="Main navigation" className="flex shrink-0 items-center gap-5">
        <p className="nav__username hidden text-sm text-muted md:block">
          Hello{" "}
          <span className="font-semibold text-ink">
            {user?.firstName} {user?.lastName}
          </span>
        </p>
        <div
          ref={menuRef}
          className="relative"
          onBlur={(event) => {
            if (!event.currentTarget.contains(event.relatedTarget)) setIsExpanded(false);
          }}
          onKeyDown={(event) => {
            if (event.key === "Escape") {
              setIsExpanded(false);
              triggerRef.current?.focus();
            }
          }}
        >
          <button
            ref={triggerRef}
            type="button"
            aria-expanded={isExpanded}
            aria-controls="navigation-links"
            className="nav__actions flex min-h-11 items-center gap-3 rounded-lg border border-line px-4 py-2 text-sm font-semibold hover:bg-elevated"
            onClick={() => setIsExpanded((current) => !current)}
          >
            Menu{" "}
            <AiOutlineDown
              aria-hidden="true"
              className={`transition-transform ${isExpanded ? "rotate-180" : ""}`}
            />
          </button>
          {isExpanded && (
            <div
              id="navigation-links"
              className="absolute right-0 z-30 mt-2 w-56 rounded-xl border border-line bg-surface p-2 shadow-card"
            >
              <Link className={menuItem} to={ROUTES.HOME} onClick={() => setIsExpanded(false)}>
                Posts
              </Link>
              <button
                className={`${menuItem} text-mint`}
                type="button"
                onClick={() => {
                  setIsExpanded(false);
                  triggerRef.current?.focus();
                  dispatch({ type: "SHOW_MODAL", modal: "createPost" });
                }}
              >
                Create Post
              </button>
              {isAdmin && (
                <Link className={menuItem} to={ROUTES.ADMIN} onClick={() => setIsExpanded(false)}>
                  Dashboard
                </Link>
              )}
              <Link className={menuItem} to={ROUTES.CHAT} onClick={() => setIsExpanded(false)}>
                Chat
              </Link>
              <button
                className={`${menuItem} text-muted`}
                type="button"
                onClick={() => {
                  setIsExpanded(false);
                  logout();
                }}
              >
                Logout
              </button>
            </div>
          )}
        </div>
      </nav>
      <CreatePostForm />
    </>
  );
};
