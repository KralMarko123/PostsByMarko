import { React, useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { modalTransitionDuration } from "../../../constants/misc";
import { useSignalR } from "../../../custom/useSignalR";
import PostsService from "../../../api/PostsService";
import Button from "../../Helper/Button/Button";
import Modal from "../../Helper/Modal";
import AppContext from "../../../context/AppContext";
import { HelperFunctions } from "../../../util/helperFunctions";
import "./CreatePostForm.css";
import "../Form.css";

const CreatePostForm = () => {
	const appContext = useContext(AppContext);
	const createPostForm = FORMS.CREATE_POST_FORM;
	const [newPostData, setNewPostData] = useState({
		title: "",
		content: "",
	});
	const [errorMessage, setErrorMessage] = useState("");
	const [confirmationalMessage, setConfirmationalMessage] = useState("");
	const [isLoading, setIsLoading] = useState(false);
	const { user } = useAuth();
	const { sendMessage } = useSignalR();

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "createPost" });
		setTimeout(() => {
			setErrorMessage("");
			setNewPostData({ title: "", content: "" });
		}, modalTransitionDuration);
	};

	const noEmptyFields = () => {
		if (!HelperFunctions.noEmptyFields(newPostData)) {
			setErrorMessage("Fields can't be empty");
			return false;
		} else return true;
	};

	const onSubmit = async () => {
		if (noEmptyFields()) {
			setErrorMessage("");
			setIsLoading(true);

			await PostsService.createPost(newPostData, user.token)
				.then((requestResult) => {
					if (requestResult.statusCode === 201) {
						sendMessage("Created Post", true);
						setConfirmationalMessage(requestResult.message);
					} else {
						setErrorMessage(requestResult.message);
					}

					appContext.dispatch({
						type: "CREATED_POST",
						post: response.newPost,
					});

					setTimeout(() => {
						onClose();
					}, 1000);
				})
				.finally(setIsLoading(false));
		}
	};

	return (
		<Modal
			isShown={appContext.modalVisibility.createPost}
			title={createPostForm.formTitle}
			onClose={() => onClose()}
		>
			<form method="POST" className="form create-post">
				<h1 className="form-title">Create A Post</h1>
				{createPostForm.formGroups.map((group) => (
					<div key={group.id} className={`form-group ${group.type === "textarea" ? "text" : ""}`}>
						{group.type === "textarea" ? (
							<textarea
								id={group.id}
								className="input input-text"
								onChange={(e) =>
									setNewPostData({ ...newPostData, [`${group.id}`]: e.currentTarget.value })
								}
								placeholder={`What do you want to share, ${user.firstName}?`}
							/>
						) : (
							<input
								id={group.id}
								type={group.type}
								className="input"
								onChange={(e) =>
									setNewPostData({ ...newPostData, [`${group.id}`]: e.currentTarget.value })
								}
								placeholder="What should this post be about?"
							/>
						)}
						{group.icon}
					</div>
				))}

				<div className="form-actions">
					<Button onButtonClick={() => onSubmit()} text="Submit" loading={isLoading} />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>

				{errorMessage && <p className="error">{errorMessage}</p>}
				{confirmationalMessage && <p className="success">{confirmationalMessage}</p>}
			</form>
		</Modal>
	);
};
export default CreatePostForm;
