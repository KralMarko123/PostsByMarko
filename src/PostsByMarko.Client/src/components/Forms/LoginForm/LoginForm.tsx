import { FormField } from "../FormField";
import { FormLayout } from "../FormLayout";
import { useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { Link } from "react-router-dom";
import { ROUTES } from "../../../constants/routes";
import { FORMS } from "../../../constants/forms";
import { HelperFunctions } from "../../../util/helperFunctions";
import { Button } from "../../Helper/Button/Button";
import { AuthService } from "../../../api/AuthService";
import { LoginRequest } from "@typeConfigs/auth";

export const LoginForm = () => {
  const { login } = useAuth();
  const loginForm = FORMS.LOGIN_FORM;
  const [loginRequest, setLoginRequest] = useState<LoginRequest>({
    email: "",
    password: "",
  });
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string>("");

  const noEmptyFields = () => {
    if (!HelperFunctions.noEmptyFields(loginRequest)) {
      setErrorMessage("Fields can't be empty");
      return false;
    } else return true;
  };

  const handleLogin = async () => {
    if (noEmptyFields()) {
      setErrorMessage("");
      setIsLoading(true);

      await AuthService.login(loginRequest)
        .then((loginPayload) => {
          login(loginPayload);
        })
        .catch((error) => setErrorMessage(error.message))
        .finally(() => setIsLoading(false));
    }
  };

  return (
    <FormLayout
      title="Sign In"
      description="Stay updated with the newest posts"
      onSubmit={handleLogin}
    >
      {loginForm.formGroups.map((group) => (
        <FormField
          key={group.id}
          name={group.id}
          label={group.label ?? group.placeholder}
          type={group.id === "email" ? "email" : group.type}
          placeholder={group.placeholder}
          icon={group.icon}
          autoComplete={group.id === "password" ? "current-password" : "email"}
          value={loginRequest[group.id as keyof LoginRequest]}
          disabled={isLoading}
          onChange={(value) => setLoginRequest({ ...loginRequest, [group.id]: value })}
        />
      ))}
      <div className="form-actions flex flex-col gap-3">
        <Button type="submit" text="Sign In" loading={isLoading} />
      </div>
      <Link className="link text-sm" to={ROUTES.REGISTER}>
        Haven't registered yet? Click here to create an account
      </Link>
      {errorMessage && (
        <p className="error text-sm" role="alert">
          {errorMessage}
        </p>
      )}
    </FormLayout>
  );
};
