import { FormField } from "../FormField";
import { FormLayout } from "../FormLayout";
import { useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { PostService } from "../../../api/PostService";
import { Button } from "../../Helper/Button/Button";
import { Modal } from "../../Helper/Modal/Modal";
import { AppContext } from "../../../context/AppContext";
import { HelperFunctions } from "../../../util/helperFunctions";
import { CreatePostRequest } from "@typeConfigs/post";

export const CreatePostForm = () => {
  const appContext = useContext(AppContext);
  const createPostForm = FORMS.CREATE_POST_FORM;
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { user } = useAuth();
  const [newPost, setNewPost] = useState<CreatePostRequest>({
    title: "",
    content: "",
  });

  const onClose = () => {
    appContext.dispatch({ type: "CLOSE_MODAL", modal: "createPost" });
    setErrorMessage("");
    setConfirmationalMessage("");
    setNewPost({ title: "", content: "" });
  };

  const noEmptyFields = () => {
    if (!HelperFunctions.noEmptyFields(newPost)) {
      setErrorMessage("Fields can't be empty");
      return false;
    } else return true;
  };

  const onSubmit = async () => {
    if (noEmptyFields()) {
      setErrorMessage("");
      setIsLoading(true);

      await PostService.createPost(newPost, user!.token!)
        .then((postResponse) => {
          appContext.dispatch({
            type: "CREATED_POST",
            post: postResponse,
          });

          setConfirmationalMessage("Successfully created Post!");

          setTimeout(() => {
            onClose();
          }, 1000);
        })
        .catch((error) => setErrorMessage(error.message))
        .finally(() => setIsLoading(false));
    }
  };

  return (
    <Modal title="Create post" isShown={appContext.modalVisibility.createPost} onClose={onClose}>
      <FormLayout
        title="Create post"
        description="Build & share with your friends"
        className="create-post"
        onSubmit={onSubmit}
      >
        {createPostForm.formGroups.map((group) => (
          <FormField
            key={group.id}
            name={group.id}
            label={group.label ?? group.placeholder}
            type={group.type}
            icon={group.icon}
            placeholder={
              group.type === "textarea"
                ? `What do you want to share, ${user!.firstName}?`
                : "What should the title for this post be?"
            }
            value={newPost[group.id as "title" | "content"]}
            disabled={isLoading}
            maxLength={group.type === "textarea" ? 20000 : 200}
            onChange={(value) => setNewPost({ ...newPost, [group.id]: value })}
          />
        ))}
        <div className="form-actions flex flex-wrap gap-3">
          <Button type="submit" text="Create" loading={isLoading} />
          <Button onButtonClick={onClose} text="Cancel" variant="secondary" />
        </div>
        {errorMessage && (
          <p className="error text-sm" role="alert">
            {errorMessage}
          </p>
        )}
        {confirmationalMessage && (
          <p className="success text-sm" role="status">
            {confirmationalMessage}
          </p>
        )}
      </FormLayout>
    </Modal>
  );
};
