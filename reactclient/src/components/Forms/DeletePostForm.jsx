import { React, useState } from "react";
import PostsService from "../../api/PostsService";
import Button from "../UI/Button";
import Modal from "../UI/Modal";
import "../../styles/components/Form.css";

const DeletePostForm = (props) => {
	const [message, setMessage] = useState(null);

	const onClose = () => {
		props.onClose();
		setMessage(null);
	};

	const onDelete = async () => {
		await PostsService.deletePostById(props.postId)
			.then(() => {
				setMessage({
					type: "success",
					message: "Post deleted successfully.",
				});
				setTimeout(() => {
					onClose();
				}, 2000);
				setTimeout(() => {
					props.onPostDeleted();
				}, 2300);
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
