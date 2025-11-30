import { React, useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { PostService } from "../../../api/PostService";
import Button from "../../Helper/Button/Button";
import Modal from "../../Helper/Modal/Modal";
import AppContext from "../../../context/AppContext";
import "../Form.css";
import "./DeletePostForm.css";

const DeletePostForm = () => {
  const appContext = useContext(AppContext);
  const [errorMessage, setErrorMessage] = useState("");
  const [confirmationalMessage, setConfirmationalMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const { user } = useAuth();

  const onDelete = async () => {
    setIsLoading(true);
    await PostService.deletePostById(
      appContext.postBeingModified.id,
      user.token
    )
      .then((response) => {
        setErrorMessage("");
        setConfirmationalMessage("Post deleted successfully");
        appContext.dispatch({
          type: "DELETED_POST",
          id: appContext.postBeingModified.id,
        });
        setTimeout(() => {
          appContext.dispatch({ type: "CLOSE_MODAL", modal: "deletePost" });
          setConfirmationalMessage("");
        }, 1000);
      })
      .catch((error) => {
        setConfirmationalMessage("");
        setErrorMessage(error.message);
      })
      .finally(() => setIsLoading(false));
  };

  return (
    <Modal
      isShown={appContext.modalVisibility.deletePost}
      title="Delete Post"
      onClose={() => onClose()}
    >
      <form method="DELETE" className="form">
        <h1 className="form-title">Are you sure?</h1>
        <p className="form-desc">Deleting this post cannot be undone</p>
        <div className="form-actions">
          <Button
            onButtonClick={() => onDelete()}
            text="Delete"
            loading={isLoading}
          />
          <Button
            onButtonClick={() =>
              appContext.dispatch({ type: "CLOSE_MODAL", modal: "deletePost" })
            }
            text="Cancel"
          />
        </div>

        {errorMessage && <p className="error">{errorMessage}</p>}
        {confirmationalMessage && (
          <p className="success">{confirmationalMessage}</p>
        )}
      </form>
    </Modal>
  );
};

export default DeletePostForm;
