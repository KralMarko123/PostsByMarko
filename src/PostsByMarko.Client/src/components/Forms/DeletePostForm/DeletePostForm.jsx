import { React, useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { useSignalR } from "../../../custom/useSignalR";
import PostsService from "../../../api/PostsService";
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
  const { sendMessage } = useSignalR();

  const onDelete = async () => {
    setIsLoading(true);
    await PostsService.deletePostById(
      appContext.postBeingModified.id,
      user.token
    )
      .then((requestResult) => {
        if (requestResult.statusCode === 200) {
          setErrorMessage("");
          setConfirmationalMessage(requestResult.message);
          sendMessage({
            message: `Deleted post with Id: ${appContext.postBeingModified.id}`,
          });
          appContext.dispatch({
            type: "DELETED_POST",
            id: appContext.postBeingModified.id,
          });

          setTimeout(() => {
            appContext.dispatch({ type: "CLOSE_MODAL", modal: "deletePost" });
            setConfirmationalMessage("");
          }, 1000);
        } else {
          setConfirmationalMessage("");
          setErrorMessage(requestResult.message);
        }
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
