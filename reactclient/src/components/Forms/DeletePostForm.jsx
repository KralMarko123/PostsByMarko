import { React, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { modalTransitionDuration } from "../../constants/misc";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import "../../styles/components/Form.css";

const DeletePostForm = (props) => {
	const [message, setMessage] = useState(null);
	const { user } = useAuth();

	const onClose = () => {
		props.onClose();
		setTimeout(() => {
			setMessage(null);
		}, modalTransitionDuration);
	};

	const onDelete = async () => {
		await PostsService.deletePostById(props.postId, user.token)
			.then(() => {
				setMessage({
					type: "success",
					message: "Post deleted successfully",
				});
				setTimeout(() => {
					onClose();
				}, 1000);
				setTimeout(() => {
					props.onPostDeleted();
				}, 1000 + modalTransitionDuration);
			})
			.catch((error) => {
				console.error(error);
				setMessage({
					type: "fail",
					message: "Error during post deletion",
				});
			});
	};

	return (
		<Modal isShown={props.isShown} title="Delete Post" message={message} onClose={() => onClose()}>
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
