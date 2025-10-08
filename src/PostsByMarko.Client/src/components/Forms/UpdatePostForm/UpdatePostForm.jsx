import { React, useContext, useState, useEffect } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { useSignalR } from "../../../custom/useSignalR";
import PostsService from "../../../api/PostsService";
import Button from "../../Helper/Button/Button";
import Modal from "../../Helper/Modal/Modal";
import AppContext from "../../../context/AppContext";
import { HelperFunctions } from "../../../util/helperFunctions";
import "./UpdatePostForm.css";
import "../Form.css";

const UpdatePostForm = () => {
  const appContext = useContext(AppContext);
  const updatePostForm = FORMS.UPDATE_POST_FORM;
  const [errorMessage, setErrorMessage] = useState("");
  const [confirmationalMessage, setConfirmationalMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const { user } = useAuth();
  const { sendMessage } = useSignalR();
  const [updatedPostData, setUpdatedPostData] = useState({
    ...appContext.postBeingModified,
  });

  useEffect(() => {
    setUpdatedPostData({ ...appContext.postBeingModified });
  }, [appContext.postBeingModified]);

  const onClose = () => {
    appContext.dispatch({ type: "CLOSE_MODAL", modal: "updatePost" });
    setErrorMessage("");
    setConfirmationalMessage("");
    setUpdatedPostData({});
  };

  const notSameData = () => {
    if (
      updatedPostData.title === appContext.postBeingModified.title &&
      updatedPostData.content === appContext.postBeingModified.content
    ) {
      setErrorMessage("You haven't made any changes");
      return false;
    } else return true;
  };

  const noEmptyFields = () => {
    if (!HelperFunctions.noEmptyFields(updatedPostData)) {
      setErrorMessage("Fields can't be empty");
      return false;
    } else return true;
  };

  const onSubmit = async () => {
    if (noEmptyFields() && notSameData()) {
      setErrorMessage("");
      setIsLoading(true);
      await PostsService.updatePost(updatedPostData, user.token)
        .then((requestResult) => {
          if (requestResult.statusCode === 200) {
            appContext.dispatch({
              type: "UPDATED_POST",
              post: updatedPostData,
            });
            sendMessage({
              message: `Updated post with Id: ${appContext.postBeingModified.id}`,
            });
            setConfirmationalMessage(requestResult.message);

            setTimeout(() => {
              onClose();
            }, 1000);
          } else setErrorMessage(requestResult.message);
        })
        .finally(() => setIsLoading(false));
    }
  };

  return (
    <Modal
      isShown={appContext.modalVisibility.updatePost}
      onClose={() => onClose()}
    >
      <form method="POST" className="form update-post">
        <h1 className="form-title">Update post</h1>
        <p className="form-desc">Make changes and keep things interesting</p>

        {updatePostForm.formGroups.map((group) => (
          <div
            key={group.id}
            className={`form-group ${group.type === "textarea" ? "text" : ""}`}
          >
            {group.type === "textarea" ? (
              <textarea
                id={group.id}
                className="input input-text"
                onChange={(e) =>
                  setUpdatedPostData({
                    ...updatedPostData,
                    [`${group.id}`]: e.currentTarget.value,
                  })
                }
                defaultValue={appContext.postBeingModified.content}
                placeholder={`What do you want to share, ${user.firstName}?`}
              />
            ) : (
              <input
                id={group.id}
                type={group.type}
                className="input"
                onChange={(e) =>
                  setUpdatedPostData({
                    ...updatedPostData,
                    [`${group.id}`]: e.currentTarget.value,
                  })
                }
                defaultValue={appContext.postBeingModified.title}
                placeholder="What should the title for this post be?"
              />
            )}
            {group.icon}
          </div>
        ))}

        <div className="form-actions">
          <Button
            onButtonClick={() => onSubmit()}
            text="Update"
            loading={isLoading}
          />
          <Button onButtonClick={() => onClose()} text="Cancel" />
        </div>

        {errorMessage && <p className="error">{errorMessage}</p>}
        {confirmationalMessage && (
          <p className="success">{confirmationalMessage}</p>
        )}
      </form>
    </Modal>
  );
};

export default UpdatePostForm;
