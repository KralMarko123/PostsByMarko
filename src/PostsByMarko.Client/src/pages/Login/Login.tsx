import { LoginForm } from "../../components/Forms/LoginForm/LoginForm";
import { Card } from "../../components/Helper/Card/Card";
import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";

export const Login = () => (
  <PageLayout className="login" authenticated={false}>
    <div className="mx-auto flex w-full max-w-lg flex-1 items-center px-5 py-10 sm:px-8">
      <Card className="w-full">
        <LoginForm />
      </Card>
    </div>
  </PageLayout>
);
