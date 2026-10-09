import type { ReactNode } from "react";
import { Logo } from "../Logo/Logo";
import { Nav } from "../Nav/Nav";
import { Footer } from "../Footer/Footer";

interface PageLayoutProps {
  children: ReactNode;
  className?: string;
  authenticated?: boolean;
}

export const PageLayout = ({ children, className = "", authenticated = true }: PageLayoutProps) => (
  <div className={`page flex min-h-dvh flex-col bg-page text-ink ${className}`}>
    <a
      href="#main-content"
      className="sr-only rounded-lg bg-mint px-4 py-2 text-page focus:not-sr-only focus:fixed focus:top-2 focus:left-2 focus:z-50"
    >
      Skip to content
    </a>
    <header className="border-b border-line/60">
      <div className="mx-auto flex min-h-20 w-full max-w-6xl items-center justify-between gap-4 px-5 sm:px-8">
        <Logo />
        {authenticated && <Nav />}
      </div>
    </header>
    <main id="main-content" className="flex w-full flex-1 flex-col">
      {children}
    </main>
    <Footer />
  </div>
);
