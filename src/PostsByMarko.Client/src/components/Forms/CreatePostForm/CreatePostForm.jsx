import { React, useContext, useState } from "react";
import { useAuth } from "../../../custom/useAuth";
import { FORMS } from "../../../constants/forms";
import { modalTransitionDuration } from "../../../constants/icons";
import { useSignalR } from "../../../custom/useSignalR";
import PostsService from "../../../api/PostsService";
import Button from "../../Helper/Button/Button";
import Modal from "../../Helper/Modal/Modal";
import AppContext from "../../../context/AppContext";
import { HelperFunctions } from "../../../util/helperFunctions";
import "./CreatePostForm.css";
import "../Form.css";
import Card from "../../Helper/Card/Card";

const CreatePostForm = () => {
	const appContext = useContext(AppContext);
	const createPostForm = FORMS.CREATE_POST_FORM;
	const [errorMessage, setErrorMessage] = useState("");
	const [confirmationalMessage, setConfirmationalMessage] = useState("");
	const [isLoading, setIsLoading] = useState(false);
	const { user } = useAuth();
	const { sendMessage } = useSignalR();
	const [newPostData, setNewPostData] = useState({
		authorId: user.userId,
		title: "",
		content: "",
	});

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "createPost" });
		setErrorMessage("");
		setConfirmationalMessage("");
		setNewPostData({ title: "", content: "" });
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
						appContext.dispatch({
							type: "CREATED_POST",
							post: requestResult.payload,
						});

						setTimeout(() => {
							onClose();
						}, 1000);
					} else {
						setErrorMessage(requestResult.message);
					}
				})
				.finally(setIsLoading(false));
		}
	};

	return (
		<Modal
			isShown={appContext.modalVisibility.createPost}
			title="Create A Post"
			onClose={() => onClose()}
		>
			<form method="POST" className="form create-post">
				<h1 className="form-title">Create post</h1>
				<p className="form-desc">Build & share with your friends</p>
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
								placeholder="What should the title for this post be?"
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
			</form>{" "}
		</Modal>
	);
};
export default CreatePostForm;
