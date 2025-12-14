import { RegisterForm } from "../../components/Forms/RegisterForm/RegisterForm";
import { Card } from "../../components/Helper/Card/Card";
import { Container } from "../../components/Layout/Container/Container";
import { Logo } from "../../components/Layout/Logo/Logo";
import "../Page.css";
import "./Register.css";

export const Register = () => {
  return (
    <div className="register page">
      <Logo />

      <Container>
        <Card>
          <RegisterForm />
        </Card>
      </Container>
    </div>
  );
};
