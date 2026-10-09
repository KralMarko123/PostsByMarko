import { FormField } from "../FormField";
import { FormLayout } from "../FormLayout";
import { useContext, useState, useEffect } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { PostService } from "../../../api/PostService";
import { HelperFunctions } from "../../../util/helperFunctions";
import { Button } from "../../Helper/Button/Button";
import { Modal } from "../../Helper/Modal/Modal";
import { AppContext } from "../../../context/AppContext";
import { UpdatePostRequest } from "@typeConfigs/post";

export const UpdatePostForm = () => {
  const { user } = useAuth();
  const appContext = useContext(AppContext);
  const updatePostForm = FORMS.UPDATE_POST_FORM;
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const [postId, setUpdatedPostId] = useState<string | null | undefined>(
    appContext.postBeingModified.id,
  );
  const [updatePostRequest, setUpdatedPostRequest] = useState<UpdatePostRequest>({
    title: appContext.postBeingModified.title,
    content: appContext.postBeingModified.content,
    hidden: appContext.postBeingModified.hidden,
  });

  useEffect(() => {
    setUpdatedPostId(appContext.postBeingModified.id);
    setUpdatedPostRequest({
      title: appContext.postBeingModified.title,
      content: appContext.postBeingModified.content,
      hidden: appContext.postBeingModified.hidden,
    });
  }, [appContext.postBeingModified, appContext.modalVisibility.updatePost]);

  const onClose = () => {
    appContext.dispatch({ type: "CLOSE_MODAL", modal: "updatePost" });
    setErrorMessage("");
    setConfirmationalMessage("");
  };

  const notSameData = () => {
    if (
      updatePostRequest.title === appContext.postBeingModified.title &&
      updatePostRequest.content === appContext.postBeingModified.content
    ) {
      setErrorMessage("You haven't made any changes");
      return false;
    } else return true;
  };

  const noEmptyFields = () => {
    if (!HelperFunctions.noEmptyFields(updatePostRequest)) {
      setErrorMessage("Fields can't be empty");
      return false;
    } else return true;
  };

  const onSubmit = async () => {
    if (noEmptyFields() && notSameData()) {
      setErrorMessage("");
      setIsLoading(true);

      await PostService.updatePost(postId!, updatePostRequest, user!.token!)
        .then((updatedPostResponse) => {
          appContext.dispatch({
            type: "UPDATED_POST",
            post: updatedPostResponse,
          });

          setConfirmationalMessage("Successfully updated Post!");

          setTimeout(() => {
            onClose();
          }, 1000);
        })
        .catch((error) => setErrorMessage(error.message))
        .finally(() => setIsLoading(false));
    }
  };

  return (
    <Modal title="Update post" isShown={appContext.modalVisibility.updatePost} onClose={onClose}>
      <FormLayout
        title="Update post"
        description="Make changes and keep things interesting"
        className="update-post"
        onSubmit={onSubmit}
      >
        {updatePostForm.formGroups.map((group) => (
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
            value={updatePostRequest[group.id as "title" | "content"]}
            disabled={isLoading}
            maxLength={group.type === "textarea" ? 20000 : 200}
            onChange={(value) => setUpdatedPostRequest({ ...updatePostRequest, [group.id]: value })}
          />
        ))}
        <div className="form-actions flex flex-wrap gap-3">
          <Button type="submit" text="Update" loading={isLoading} />
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
