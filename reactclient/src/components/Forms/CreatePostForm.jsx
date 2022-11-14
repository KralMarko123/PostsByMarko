import { React, useState, useRef } from "react";
import PostsService from "../../api/PostsService";
import Button from "../Helper/Button";
import Modal from "../Helper/Modal";
import "../../styles/components/Form.css";
import { useAuth } from "../../custom/useAuth";

const CreatePostForm = (props) => {
	const titleRef = useRef();
	const contentRef = useRef();
	const [message, setMessage] = useState(null);
	const transitionDuration = 0.25;
	const { user } = useAuth();

	const onClose = () => {
		props.onClose();
		setMessage(null);
	};

	const onSubmit = async () => {
		if (titleRef.current.value.length > 0 && contentRef.current.value.length > 0) {
			const postToCreate = {
				title: titleRef.current.value,
				content: contentRef.current.value,
			};

			await PostsService.createPost(postToCreate, user.token)
				.then(() => {
					props.onSubmit();
					setMessage({
						type: "success",
						message: "Post created successfully.",
					});
					setTimeout(() => {
						onClose();
					}, transitionDuration * 1000 + 500);
				})
				.catch((error) => {
					console.error(error.message);
					setMessage({
						type: "fail",
						message: "Error during post creation.",
					});
				});
		}

		if (titleRef.current.value.length === 0)
			titleRef.current.placeholder = "Please enter a title...";
		if (contentRef.current.value.length === 0)
			contentRef.current.placeholder = "Please add some content...";
	};

	return (
		<Modal
			isShown={props.isShown}
			title="Create Form"
			message={message}
			onClose={() => onClose()}
			duration={transitionDuration}
		>
			<form className="form">
				<div className="form__group">
					<label htmlFor="title" className="input__label">
						Title
					</label>
					<input id="title" type="text" className="input" ref={titleRef} />
				</div>
				<div className="form__group">
					<label htmlFor="content" className="input__label">
						Content
					</label>
					<textarea id="content" className="input" ref={contentRef} />
				</div>

				<div className="form__actions">
					<Button onButtonClick={() => onSubmit()} text="Submit" />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};

export default CreatePostForm;
