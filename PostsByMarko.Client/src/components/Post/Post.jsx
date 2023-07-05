import { React, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/misc";
import * as ROUTES from "../../constants/routes";
import AppContext from "../../context/AppContext";
import PostsService from "../../api/PostsService";
import Card from "../Helper/Card/Card";
import "./Post.css";

const Post = ({ postId, authorId, title, content, isHidden }) => {
	let navigate = useNavigate();
	const appContext = useContext(AppContext);
	const { user, isAdmin, isEditor } = useAuth();
	const isAuthor = authorId === user.userId;

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
		<Card>
			<div className={`post ${isHidden ? "hidden" : ""}`} onClick={() => handlePostClick()}>
				{(isAuthor || isAdmin) && (
					<>
						<span className="post-icon hide" onClick={(e) => handleHiddenToggle(e)}>
							{ICONS.EYE_ICON(isHidden)}
						</span>
					</>
				)}
				{(isAuthor || isAdmin || isEditor) && (
					<span className="post-icon update" onClick={(e) => handleModalToggle(e, "updatePost")}>
						{ICONS.PENCIL_ICON()}
					</span>
				)}
				{(isAuthor || isAdmin) && (
					<span className="post-icon delete" onClick={(e) => handleModalToggle(e, "deletePost")}>
						{ICONS.DELETE_ICON()}
					</span>
				)}
				<h1 className="post-title">{title}</h1>
				<p className="post-content">{content}</p>
			</div>
		</Card>
	);
};

export default Post;
