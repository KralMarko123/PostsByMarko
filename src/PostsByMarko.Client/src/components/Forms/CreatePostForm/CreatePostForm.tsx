import { useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { PostService } from "../../../api/PostService";
import { Button } from "../../Helper/Button/Button";
import { Modal } from "../../Helper/Modal/Modal";
import { AppContext } from "../../../context/AppContext";
import { HelperFunctions } from "../../../util/helperFunctions";
import { CreatePostRequest } from "@typeConfigs/post";
import "./CreatePostForm.css";
import "../Form.css";

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
    <Modal isShown={appContext.modalVisibility.createPost} onClose={() => onClose()}>
      <form method="POST" className="form create-post">
        <h1 className="form-title">Create post</h1>
        <p className="form-desc">Build & share with your friends</p>

        {createPostForm.formGroups.map((group) => (
          <div
            key={group.id}
            className={`form-group ${group.type === "textarea" ? "text" : ""}`}
          >
            {group.type === "textarea" ? (
              <textarea
                id={group.id}
                className="input input-text"
                onChange={(e) =>
                  setNewPost({
                    ...newPost,
                    [`${group.id}`]: e.currentTarget.value,
                  })
                }
                placeholder={`What do you want to share, ${user!.firstName}?`}
              />
            ) : (
              <input
                id={group.id}
                type={group.type}
                className="input"
                onChange={(e) =>
                  setNewPost({
                    ...newPost,
                    [`${group.id}`]: e.currentTarget.value,
                  })
                }
                placeholder="What should the title for this post be?"
              />
            )}
            {group.icon}
          </div>
        ))}

        <div className="form-actions">
          <Button onButtonClick={onSubmit} text="Create" loading={isLoading} />
          <Button onButtonClick={onClose} text="Cancel" />
        </div>

        {errorMessage && <p className="error">{errorMessage}</p>}
        {confirmationalMessage && <p className="success">{confirmationalMessage}</p>}
      </form>{" "}
    </Modal>
  );
};
