import { LoginForm } from "../../components/Forms/LoginForm/LoginForm";
import { Card } from "../../components/Helper/Card/Card";
import { Container } from "../../components/Layout/Container/Container";
import { Logo } from "../../components/Layout/Logo/Logo";
import "../Page.css";
import "./Login.css";

export const Login = () => {
  return (
    <div className="login page">
      <Logo />

      <Container>
        <Card>
          <LoginForm />
        </Card>
      </Container>
    </div>
  );
};
