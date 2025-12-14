import { useState, useEffect } from "react";
import { CreatePostForm } from "../../Forms/CreatePostForm/CreatePostForm";
import { DesktopNav } from "../DesktopNav/DesktopNav";
import { MobileNav } from "../MobileNav/MobileNav";

export const Nav = () => {
  const [width, setWidth] = useState<number>(window.innerWidth);

  const handleWindowResize = () => {
    setWidth(window.innerWidth);
  };

  useEffect(() => {
    window.addEventListener("resize", handleWindowResize);
    return () => window.removeEventListener("resize", handleWindowResize);
  }, []);

  return (
    <>
      {width <= 1199 ? <MobileNav /> : <DesktopNav />}
      <CreatePostForm />
    </>
  );
};
