import { useState } from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import { FORMS } from "../../../constants/forms";
import { HelperFunctions } from "../../../util/helperFunctions";
import { Button } from "../../Helper/Button/Button";
import { AuthService } from "../../../api/AuthService";
import { RegisterRequest } from "@typeConfigs/auth";
import "./RegisterForm.css";
import "../Form.css";

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
    let isValidRegistration =
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
    <form action="POST" className="form">
      <h1 className="form-title">Sign Up</h1>
      <p className="form-desc">Start sharing today</p>
      {registerForm.formGroups.map((group) => (
        <div key={group.id} className="form-group">
          <input
            id={group.id}
            type={group.type}
            className="input"
            placeholder={group.placeholder}
            onChange={(e) =>
              setRegisterRequest({
                ...registerRequest,
                [`${group.id}`]: e.currentTarget.value,
              })
            }
          />
          {group.icon!({})}
        </div>
      ))}

      <div className="form-actions">
        <Button onButtonClick={handleRegister} text="Sign Up" loading={isLoading} />
      </div>

      <p className="link" onClick={() => navigate(ROUTES.LOGIN)}>
        Already have an account? Click here to sign in
      </p>

      {errorMessage && <p className="error">{errorMessage}</p>}
    </form>
  ) : (
    <div className="form confirmational">
      <h1 className="form-title">Successfully Registered!</h1>
      <p className="form-desc">
        Please check your email to confirm your account first. You can click on the button
        below to sign in
      </p>
      <Button text={"Sign In"} onButtonClick={() => navigate(ROUTES.LOGIN)} />
    </div>
  );
};
