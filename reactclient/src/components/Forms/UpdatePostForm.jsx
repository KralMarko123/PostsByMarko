import { React, useState, useRef } from "react";
import PostsService from "../../api/PostsService";
import Button from "../UI/Button";
import Modal from "../UI/Modal";
import "../../styles/components/Form.css";

const UpdatePostForm = (props) => {
	const titleRef = useRef();
	const contentRef = useRef();
	const [message, setMessage] = useState(null);

	const onClose = () => {
		props.onClose();
		setMessage(null);
	};

	const onSubmit = async () => {
		if (titleRef.current.value === props.title && contentRef.current.value === props.content) {
			setMessage({
				message: "Can't update with same data.",
				type: "fail",
			});
			return;
		}

		if (titleRef.current.value.length > 0 && contentRef.current.value.length > 0) {
			const postToUpdate = {
				postId: props.postId,
				title: titleRef.current.value,
				content: contentRef.current.value,
			};

			await PostsService.updatePost(postToUpdate)
				.then(() => {
					props.onSubmit(postToUpdate);
					setMessage({
						type: "success",
						message: "Post updated successfully.",
					});
					setTimeout(() => {
						onClose();
					}, 2000);
				})
				.catch((error) => {
					console.error(error);
					setMessage({
						type: "fail",
						message: "Error during post update.",
					});
				});
		}

		if (titleRef.current.value.length === 0)
			titleRef.current.placeholder = "Please enter a title...";

		if (contentRef.current.value.length === 0)
			contentRef.current.placeholder = "Please add some content...";
	};

	return (
		<Modal isShown={props.isShown} title="Update Form" message={message} onClose={() => onClose()}>
			<form className="form">
				<div className="form__group">
					<label htmlFor="title" className="input__label">
						Title
					</label>
					<input
						id="title"
						type="text"
						className="input"
						ref={titleRef}
						defaultValue={props.title}
					/>
				</div>
				<div className="form__group">
					<label htmlFor="content" className="input__label">
						Content
					</label>
					<textarea id="content" className="input" ref={contentRef} defaultValue={props.content} />
				</div>

				<div className="form__actions">
					<Button onButtonClick={() => onSubmit()} text="Submit" />
					<Button onButtonClick={() => onClose()} text="Cancel" />
				</div>
			</form>
		</Modal>
	);
};

export default UpdatePostForm;
