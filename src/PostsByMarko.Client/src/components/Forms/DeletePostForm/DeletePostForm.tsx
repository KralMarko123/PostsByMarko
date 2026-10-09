import { FormLayout } from "../FormLayout";
import { useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { PostService } from "../../../api/PostService";
import { Button } from "../../Helper/Button/Button";
import { Modal } from "../../Helper/Modal/Modal";
import { AppContext } from "../../../context/AppContext";

export const DeletePostForm = () => {
  const appContext = useContext(AppContext);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { user } = useAuth();

  const onClose = () => {
    appContext.dispatch({ type: "CLOSE_MODAL", modal: "deletePost" });
    setErrorMessage("");
    setConfirmationalMessage("");
    setIsLoading(false);
  };

  const onDelete = async () => {
    setIsLoading(true);
    await PostService.deletePostById(appContext.postBeingModified.id!, user!.token!)
      .then(() => {
        setErrorMessage("");
        setConfirmationalMessage("Post deleted successfully");

        appContext.dispatch({
          type: "DELETED_POST",
          id: appContext.postBeingModified.id!,
        });

        setTimeout(() => onClose(), 1000);
      })
      .catch((error) => {
        setConfirmationalMessage("");
        setErrorMessage(error.message);
      })
      .finally(() => setIsLoading(false));
  };

  return (
    <Modal title="Delete post" isShown={appContext.modalVisibility.deletePost} onClose={onClose}>
      <FormLayout
        title="Are you sure?"
        description="Deleting this post cannot be undone"
        onSubmit={onDelete}
      >
        <div className="form-actions flex flex-wrap gap-3">
          <Button type="submit" text="Delete" loading={isLoading} variant="danger" />
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
