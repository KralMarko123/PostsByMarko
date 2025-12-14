import { Container } from "../../components/Layout/Container/Container";
import { Logo } from "../../components/Layout/Logo/Logo";
import { Button } from "../../components/Helper/Button/Button";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";
import "../Page.css";
import "./NotFound.css";

export const NotFound = () => {
  const navigate = useNavigate();

  return (
    <div className="page notFound">
      <Logo />
      <Container title="Not Found" desc="Looks like you've wandered off somewhere">
        <Button
          text={"Click here to go to the homepage"}
          onButtonClick={() => navigate(ROUTES.HOME)}
        />
      </Container>
    </div>
  );
};
