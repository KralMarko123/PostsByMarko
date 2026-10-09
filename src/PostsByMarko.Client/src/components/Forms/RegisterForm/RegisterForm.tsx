import { FormField } from "../FormField";
import { FormLayout } from "../FormLayout";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import { FORMS } from "../../../constants/forms";
import { HelperFunctions } from "../../../util/helperFunctions";
import { Button } from "../../Helper/Button/Button";
import { AuthService } from "../../../api/AuthService";
import { RegisterRequest } from "@typeConfigs/auth";

export const RegisterForm = () => {
  const navigate = useNavigate();
  const registerForm = FORMS.REGISTER_FORM;
  const [registerRequest, setRegisterRequest] = useState<RegisterRequest>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isRegistered, setIsRegistered] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string>("");

  const noEmptyFields = () => {
    if (!HelperFunctions.noEmptyFields(registerRequest)) {
      setErrorMessage("Fields can't be empty");
      return false;
    } else return true;
  };

  const isValidEmail = () => {
    if (!/^\S+@\S+\.\S+$/.test(registerRequest.email)) {
      setErrorMessage("Email must be a valid email address");
      return false;
    } else return true;
  };

  const isValidPassword = () => {
    const isValidPassword = HelperFunctions.isValidPassword(registerRequest.password);

    if (isValidPassword !== true) {
      setErrorMessage(isValidPassword);
    } else return isValidPassword;
  };

  const arePasswordMatching = () => {
    if (registerRequest.password !== registerRequest.confirmPassword) {
      setErrorMessage("Passwords do not match");
      return false;
    } else return true;
  };

  const handleRegister = async () => {
    const isValidRegistration =
      noEmptyFields() && isValidEmail() && isValidPassword() && arePasswordMatching();

    if (isValidRegistration) {
      setErrorMessage("");
      setIsLoading(true);

      await AuthService.register(registerRequest)
        .then(() => setIsRegistered(true))
        .catch((error) => setErrorMessage(error.message))
        .finally(() => setIsLoading(false));
    }
  };

  return !isRegistered ? (
    <FormLayout title="Sign Up" description="Start sharing today" onSubmit={handleRegister}>
      {registerForm.formGroups.map((group) => (
        <FormField
          key={group.id}
          name={group.id}
          label={group.label ?? group.placeholder}
          type={group.id === "email" ? "email" : group.type}
          placeholder={group.placeholder}
          icon={group.icon}
          autoComplete={
            group.type === "password"
              ? "new-password"
              : group.id === "firstName"
                ? "given-name"
                : group.id === "lastName"
                  ? "family-name"
                  : "email"
          }
          value={registerRequest[group.id as keyof RegisterRequest]}
          disabled={isLoading}
          onChange={(value) => setRegisterRequest({ ...registerRequest, [group.id]: value })}
        />
      ))}
      <div className="form-actions flex flex-col gap-3">
        <Button type="submit" text="Sign Up" loading={isLoading} />
      </div>
      <Link className="link text-sm" to={ROUTES.LOGIN}>
        Already have an account? Click here to sign in
      </Link>
      {errorMessage && (
        <p className="error text-sm" role="alert">
          {errorMessage}
        </p>
      )}
    </FormLayout>
  ) : (
    <div className="form confirmational flex flex-col gap-5 p-6 sm:p-8" role="status">
      <h1 className="form-title font-display text-3xl font-medium">Successfully Registered!</h1>
      <p className="form-desc text-muted">
        Please check your email to confirm your account first. You can click on the button below to
        sign in
      </p>
      <Button text="Sign In" onButtonClick={() => navigate(ROUTES.LOGIN)} />
    </div>
  );
};
