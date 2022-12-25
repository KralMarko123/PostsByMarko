import { React, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { FORMS } from "../../constants/forms";
import { modalTransitionDuration } from "../../constants/misc";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import "../../styles/components/Form.css";

const CreatePostForm = (props) => {
	const createPostForm = FORMS.createPostForm;
	const [newPostData, setNewPostData] = useState({
		title: "",
		content: "",
	});
	const [message, setMessage] = useState(null);
	const { user } = useAuth();

	const onClose = () => {
		props.onClose();
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
		let isValidPost = !checkForEmptyFields();
		if (isValidPost) {
			const postToCreate = {
				title: newPostData.title,
				content: newPostData.content,
			};

			await PostsService.createPost(postToCreate, user.token)
				.then(() => {
					props.onSubmit();
					setMessage({
						type: "success",
						message: "Post created successfully",
					});
					setTimeout(() => {
						onClose();
					}, 1000);
				})
				.catch((error) => {
					console.error(error.message);
					setMessage({
						type: "fail",
						message: "Error during post creation",
					});
				});
		}
	};

	return (
		<Modal
			isShown={props.isShown}
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
					<Button onButtonClick={() => onSubmit()} text="Submit" />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};
export default CreatePostForm;
