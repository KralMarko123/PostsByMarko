import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";
import { Container } from "../../components/Layout/Container/Container";
import { Button } from "../../components/Helper/Button/Button";
import { useNavigate } from "react-router";
import { ROUTES } from "../../constants/routes";

export const NotFound = () => {
  const navigate = useNavigate();

  return (
    <PageLayout className="notFound" authenticated={false}>
      <Container title="Not Found" desc="Looks like you've wandered off somewhere">
        <Button
          text={"Click here to go to the homepage"}
          onButtonClick={() => navigate(ROUTES.HOME)}
        />
      </Container>
    </PageLayout>
  );
};
