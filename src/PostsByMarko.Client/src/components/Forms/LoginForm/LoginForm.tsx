import { useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import { FORMS } from "../../../constants/forms";
import { HelperFunctions } from "../../../util/helperFunctions";
import { Button } from "../../Helper/Button/Button";
import { AuthService } from "../../../api/AuthService";
import { LoginRequest } from "@typeConfigs/auth";
import "./LoginForm.css";
import "../Form.css";

export const LoginForm = () => {
  const { login } = useAuth();
  const loginForm = FORMS.LOGIN_FORM;
  const navigate = useNavigate();
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
        .then((loginPayload) => login(loginPayload))
        .catch((error) => setErrorMessage(error.message))
        .finally(() => setIsLoading(false));
    }
  };

  return (
    <form action="POST" className="form">
      <h1 className="form-title">Sign In</h1>
      <p className="form-desc">Stay updated with the newest posts</p>
      {loginForm.formGroups.map((group) => (
        <div key={group.id} className="form-group">
          <input
            id={group.id}
            type={group.type}
            className="input"
            placeholder={group.placeholder}
            onChange={(e) =>
              setLoginRequest({
                ...loginRequest,
                [`${group.id}`]: e.currentTarget.value,
              })
            }
          />
          {group.icon!({})}
        </div>
      ))}

      <div className="form-actions">
        <Button onButtonClick={handleLogin} text="Sign In" loading={isLoading} />
      </div>

      <p className="link" onClick={() => navigate(ROUTES.REGISTER)}>
        Haven't registered yet? Click here to create an account
      </p>

      {errorMessage && <p className="error">{errorMessage}</p>}
    </form>
  );
};
