import { React, useContext, useState, useEffect } from "react";
import { useAuth } from "../../custom/useAuth";
import { FORMS } from "../../constants/forms";
import { modalTransitionDuration } from "../../constants/misc";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import AppContext from "../../context/AppContext";
import "../../styles/components/Form.css";

const UpdatePostForm = () => {
	const appContext = useContext(AppContext);
	const updatePostForm = FORMS.updatePostForm;
	const [newPostData, setNewPostData] = useState({ ...appContext.postBeingModified });
	const [message, setMessage] = useState(null);
	const { user } = useAuth();
	const { sendMessage } = useSignalR();

	useEffect(() => {
		setNewPostData({ ...appContext.postBeingModified });
	}, [appContext.postBeingModified]);

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "updatePost" });
		setTimeout(() => {
			setMessage(null);
		}, modalTransitionDuration);
	};

	const checkForSameData = () => {
		let hasSameData = false;

		if (
			newPostData.title === appContext.postBeingModified.title &&
			newPostData.content === appContext.postBeingModified.content
		) {
			setMessage({
				message: "Can't update with same data",
				type: "fail",
			});
			hasSameData = true;
		}

		return hasSameData;
	};

	const checkForEmptyFields = () => {
		let hasEmptyField = false;

		if (newPostData.title === "") {
			hasEmptyField = true;
			setMessage({
				type: "fail",
				message: "Title can't be empty",
			});
		}

		if (newPostData.content === "") {
			hasEmptyField = true;
			setMessage({
				type: "fail",
				message: "Content can't be empty",
			});
		}

		return hasEmptyField;
	};

	const onSubmit = async () => {
		let isValidUpdate = !checkForEmptyFields() && !checkForSameData();

		if (isValidUpdate) {
			await PostsService.updatePost(newPostData, user.token)
				.then(() => {
					sendMessage("Updated Post");
					setMessage({
						type: "success",
						message: "Post updated successfully",
					});
					setTimeout(() => {
						onClose();
					}, 1000);
				})
				.catch((error) => {
					setMessage({
						type: "fail",
						message: "Error during post update",
					});
				});
		}
	};

	return (
		<Modal
			isShown={appContext.modalVisibility.updatePost}
			title={updatePostForm.formTitle}
			message={message}
			onClose={() => onClose()}
		>
			<form method={updatePostForm.action} className="form">
				{updatePostForm.formGroups.map((group) => (
					<div key={group.id} className={`form__group ${group.type === "textarea" ? "text" : ""}`}>
						{group.icon}
						{group.type === "textarea" ? (
							<textarea
								id={group.id}
								className="input"
								onChange={(e) =>
									setNewPostData({ ...newPostData, [`${group.id}`]: e.currentTarget.value })
								}
								defaultValue={appContext.postBeingModified.content}
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
								defaultValue={appContext.postBeingModified.title}
								placeholder={group.placeholder}
							/>
						)}
					</div>
				))}

				<div className="form__actions">
					<Button onButtonClick={() => onSubmit()} text="Submit" />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};

export default UpdatePostForm;
