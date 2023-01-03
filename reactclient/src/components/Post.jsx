import { React, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../custom/useAuth";
import { ICONS } from "../constants/misc";
import * as ROUTES from "../constants/routes";
import AppContext from "../context/AppContext";
import PostsService from "../api/PostsService";
import "../styles/components/Post.css";

const Post = ({ postId, authorId, title, content, isHidden }) => {
	let navigate = useNavigate();
	const appContext = useContext(AppContext);
	const { user } = useAuth();
	const isAuthor = authorId === user.userId;
	const isAdmin = user.roles.includes("Admin");
	const isEditor = user.roles.includes("Editor");

	const handlePostClick = () => {
		navigate(`.${ROUTES.DETAILS_PREFIX}/${postId}`);
	};

	const handleModalToggle = (e, modalToToggle) => {
		e.stopPropagation();
		appContext.dispatch({
			type: "MODIFYING_POST",
			post: { postId: postId, title: title, content: content },
		});
		appContext.dispatch({ type: "SHOW_MODAL", modal: modalToToggle });
	};

	const handleHiddenToggle = async (e) => {
		e.stopPropagation();
		await PostsService.togglePostVisibility(postId, user.token)
			.then(() => appContext.dispatch({ type: "TOGGLE_POST_HIDDEN", postId: postId }))
			.catch((error) => console.log(error.message));
	};

	return (
		<div className={`post ${isHidden ? "hidden" : ""}`} onClick={() => handlePostClick()}>
			{(isAuthor || isAdmin) && (
				<>
					<span className="post__icon post__hidden" onClick={(e) => handleHiddenToggle(e)}>
						{ICONS.EYE_ICON(isHidden)}
					</span>
				</>
			)}
			{(isAuthor || isAdmin || isEditor) && (
				<span
					className="post__icon post__update"
					onClick={(e) => handleModalToggle(e, "updatePost")}
				>
					&#9998;
				</span>
			)}
			{(isAuthor || isAdmin) && (
				<span
					className="post__icon post__delete"
					onClick={(e) => handleModalToggle(e, "deletePost")}
				>
					&times;
				</span>
			)}
			<h1 className="post__title">{title}</h1>
			<p className="post__content">{content}</p>
		</div>
	);
};

export default Post;
