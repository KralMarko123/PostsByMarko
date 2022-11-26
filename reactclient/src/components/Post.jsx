import { React, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../custom/useAuth";
import DeletePostForm from "./Forms/DeletePostForm";
import UpdatePostForm from "./Forms/UpdatePostForm";
import * as ROUTES from "../constants/routes";
import "../styles/components/Post.css";

const Post = ({ postId, title, content, onPostDeleted, onPostUpdated }) => {
	let navigate = useNavigate();
	const [showDeleteForm, setShowDeleteForm] = useState(false);
	const [showUpdateForm, setShowUpdateForm] = useState(false);
	const { user } = useAuth();
	const isAdmin = user.userDetails.userRoles.includes("Admin");
	const isEditor = user.userDetails.userRoles.includes("Editor");

	const handlePostClick = () => {
		navigate(`.${ROUTES.DETAILS_PREFIX}/${postId}`);
	};

	const handlePostDelete = (e) => {
		e.stopPropagation();
		setShowDeleteForm(true);
	};

	const handlePostUpdate = (e) => {
		e.stopPropagation();
		setShowUpdateForm(true);
	};

	return (
		<>
			<div className="post" onClick={() => handlePostClick()}>
				{(isAdmin || isEditor) && (
					<span className="post__icon post__update" onClick={(e) => handlePostUpdate(e)}>
						<p>&#9998;</p>
					</span>
				)}
				{isAdmin && (
					<span className="post__icon post__delete" onClick={(e) => handlePostDelete(e)}>
						<p>&times;</p>
					</span>
				)}
				<span className="post__icon post__id">{postId}</span>
				<h1 className="post__title">{title}</h1>
				<p className="post__content">{content}</p>
			</div>

			<UpdatePostForm
				isShown={showUpdateForm}
				onSubmit={(updatedPost) => onPostUpdated(updatedPost)}
				onClose={() => setShowUpdateForm(false)}
				postId={postId}
				title={title}
				content={content}
			/>

			<DeletePostForm
				isShown={showDeleteForm}
				postId={postId}
				onClose={() => setShowDeleteForm(false)}
				onPostDeleted={() => onPostDeleted(postId)}
			/>
		</>
	);
};

export default Post;
