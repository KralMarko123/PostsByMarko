import { React, useContext, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { FORMS } from "../../constants/forms";
import { modalTransitionDuration } from "../../constants/misc";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import AppContext from "../../context/AppContext";
import "../../styles/components/Form.css";

const CreatePostForm = () => {
	const appContext = useContext(AppContext);
	const createPostForm = FORMS.createPostForm;
	const [newPostData, setNewPostData] = useState({
		title: "",
		content: "",
	});
	const [message, setMessage] = useState(null);
	const [isLoading, setIsLoading] = useState(false);
	const { user } = useAuth();
	const { sendMessage } = useSignalR();

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "createPost" });
		setTimeout(() => {
			setMessage(null);
			setNewPostData({ title: "", content: "" });
		}, modalTransitionDuration);
	};

	const checkForEmptyFields = () => {
		let hasEmptyField = false;

		if (newPostData.title === "") {
			hasEmptyField = true;
			setMessage({
				isSuccessful: false,
				message: "Title can't be empty",
			});
		}

		if (newPostData.content === "") {
			hasEmptyField = true;
			setMessage({
				isSuccessful: false,
				message: "Content can't be empty",
			});
		}

		return hasEmptyField;
	};

	const onSubmit = async () => {
		let isValidPost = !checkForEmptyFields();
		if (isValidPost) {
			setIsLoading(true);

			const postToCreate = {
				title: newPostData.title,
				content: newPostData.content,
			};

			await PostsService.createPost(postToCreate, user.token)
				.then((response) => {
					sendMessage("Created Post", true);
					setMessage({ isSuccessful: true, message: response.message });
					appContext.dispatch({
						type: "CREATED_POST",
						post: response.newPost,
					});

					setTimeout(() => {
						onClose();
					}, 1000);
				})
				.catch((error) =>
					setMessage({
						isSuccessful: false,
						message: error.message,
					})
				)
				.finally(setIsLoading(false));
		}
	};

	return (
		<Modal
			isShown={appContext.modalVisibility.createPost}
			title={createPostForm.formTitle}
			message={message}
			onClose={() => onClose()}
		>
			<form method={createPostForm.action} className="form">
				{createPostForm.formGroups.map((group) => (
					<div key={group.id} className={`form__group ${group.type === "textarea" ? "text" : ""}`}>
						{group.icon}
						{group.type === "textarea" ? (
							<textarea
								id={group.id}
								className="input"
								onChange={(e) =>
									setNewPostData({ ...newPostData, [`${group.id}`]: e.currentTarget.value })
								}
								placeholder={group.placeholder}
							/>
						) : (
							<input
								id={group.id}
								type={group.type}
								className="input"
								onChange={(e) =>
									setNewPostData({ ...newPostData, [`${group.id}`]: e.currentTarget.value })
								}
								placeholder={group.placeholder}
							/>
						)}
					</div>
				))}

				<div className="form__actions">
					<Button onButtonClick={() => onSubmit()} text="Submit" loading={isLoading} />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};
export default CreatePostForm;
