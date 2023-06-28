import { React, useContext, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { modalTransitionDuration } from "../../constants/misc";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button/Button";
import Modal from "../Helper/Modal";
import AppContext from "../../context/AppContext";

const DeletePostForm = () => {
	const appContext = useContext(AppContext);
	const [message, setMessage] = useState(null);
	const [isLoading, setIsLoading] = useState(false);
	const { user } = useAuth();
	const { sendMessage } = useSignalR();

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "deletePost" });
		setTimeout(() => {
			setMessage(null);
		}, modalTransitionDuration);
	};

	const onDelete = async () => {
		setIsLoading(true);
		await PostsService.deletePostById(appContext.postBeingModified.postId, user.token)
			.then((response) => {
				setMessage(response);

				setTimeout(() => {
					onClose();
				}, 1000);

				setTimeout(() => {
					sendMessage("Deleted Post");
					appContext.dispatch({
						type: "DELETED_POST",
						postId: appContext.postBeingModified.postId,
					});
				}, 1000 + modalTransitionDuration);
			})
			.catch((error) =>
				setMessage({
					isSuccessful: false,
					message: error.message,
				})
			)
			.finally(() => setIsLoading(false));
	};

	return (
		<Modal
			isShown={appContext.modalVisibility.deletePost}
			title="Delete Post"
			message={message}
			onClose={() => onClose()}
		>
			<form className="form">
				<h1 className="form__confirmational">Are you sure you want to delete this post?</h1>
				<div className="form__actions">
					<Button onButtonClick={() => onDelete()} text="Submit" loading={isLoading} />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};

export default DeletePostForm;
