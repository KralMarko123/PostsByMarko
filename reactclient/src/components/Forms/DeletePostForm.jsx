import { React, useState } from "react";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import "../../styles/components/Form.css";
import { useAuth } from "../../custom/useAuth";

const DeletePostForm = (props) => {
	const [message, setMessage] = useState(null);
	const transitionDuration = 0.25;
	const { user } = useAuth();

	const onClose = () => {
		props.onClose();
		setMessage(null);
	};

	const onDelete = async () => {
		await PostsService.deletePostById(props.postId, user.token)
			.then(() => {
				setMessage({
					type: "success",
					message: "Post deleted successfully.",
				});
				setTimeout(() => {
					onClose();
				}, transitionDuration * 1000 + 500);
				setTimeout(() => {
					props.onPostDeleted();
				}, transitionDuration * 2 * 1000 + 500);
			})
			.catch((error) => {
				console.error(error);
				setMessage({
					type: "fail",
					message: "Error during post deletion.",
				});
			});
	};

	return (
		<Modal
			isShown={props.isShown}
			title="Delete Post"
			message={message}
			onClose={() => onClose()}
			duration={transitionDuration}
		>
			<form className="form">
				<h1 className="form__confirmational">Are you sure you want to delete this post?</h1>
				<div className="form__actions">
					<Button onButtonClick={() => onDelete()} text="Delete" />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};

export default DeletePostForm;
