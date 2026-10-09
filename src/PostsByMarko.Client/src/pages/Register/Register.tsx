import { RegisterForm } from "../../components/Forms/RegisterForm/RegisterForm";
import { Card } from "../../components/Helper/Card/Card";
import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";

export const Register = () => (
  <PageLayout className="register" authenticated={false}>
    <div className="mx-auto flex w-full max-w-lg flex-1 items-center px-5 py-10 sm:px-8">
      <Card className="w-full">
        <RegisterForm />
      </Card>
    </div>
  </PageLayout>
);
