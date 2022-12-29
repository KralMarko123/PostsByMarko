import { React, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../custom/useAuth";
import * as ROUTES from "../constants/routes";
import AppContext from "../context/AppContext";
import "../styles/components/Post.css";

const Post = ({ postId, authorId, title, content }) => {
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

	return (
		<>
			<div className="post" onClick={() => handlePostClick()}>
				{(isAuthor || isAdmin || isEditor) && (
					<span
						className="post__icon post__update"
						onClick={(e) => handleModalToggle(e, "updatePost")}
					>
						<p>&#9998;</p>
					</span>
				)}
				{(isAuthor || isAdmin) && (
					<span
						className="post__icon post__delete"
						onClick={(e) => handleModalToggle(e, "deletePost")}
					>
						<p>&times;</p>
					</span>
				)}
				<h1 className="post__title">{title}</h1>
				<p className="post__content">{content}</p>
			</div>
		</>
	);
};

export default Post;
