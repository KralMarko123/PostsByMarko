import { React, useContext, useEffect, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import Modal from "../Helper/Modal";
import AppContext from "../../context/AppContext";
import Button from "../Helper/Button";
import UsersService from "../../api/UsersService";
import PostsService from "../../api/PostsService";
import "../../styles/components/ToggleUserToPost.css";

const ToggleUserForPost = () => {
	const { user } = useAuth();
	const [allUsers, setAllUsers] = useState([]);
	const appContext = useContext(AppContext);

	const getUsers = async () => {
		await UsersService.getAllUsers(user.token).then((usersFromServer) =>
			setAllUsers(usersFromServer)
		);
	};

	const isAllowedUser = (username) => {
		return appContext.posts
			.find((p) => p.postId == appContext.postBeingModified.postId)
			?.allowedUsers.includes(username);
	};

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "addUserToPost" });
	};

	const toggleUserForPost = async (postId, username, isAdded) => {
		await PostsService.toggleUserForPost(postId, username, user.token)
			.then(() => {
				appContext.dispatch({
					type: "TOGGLED_USER",
					username: username,
					isAdded: isAdded,
					postId: postId,
				});
			})
			.catch((error) => console.log(error));
	};

	useEffect(() => {
		getUsers();
	}, []);

	useEffect(() => {}, [appContext.posts]);

	return (
		<Modal
			isShown={appContext.modalVisibility.addUserToPost}
			title="Add Users To Post"
			onClose={() => onClose()}
		>
			<div className="allowed__users__container">
				<ul className="users__list">
					{allUsers.map((u) => (
						<li className={`user ${isAllowedUser(u) ? "allowed" : ""}`} key={u}>
							<p>{u}</p>
							{!isAllowedUser(u) && (
								<span
									className="user__add"
									onClick={() => toggleUserForPost(appContext.postBeingModified.postId, u, true)}
								>
									<p>&#x2713;</p>
									<p>Add</p>
								</span>
							)}
							{isAllowedUser(u) && (
								<span
									className="user__remove"
									onClick={() => toggleUserForPost(appContext.postBeingModified.postId, u, false)}
								>
									<p>&times;</p>
									<p>Remove</p>
								</span>
							)}
						</li>
					))}
				</ul>
				<Button onButtonClick={() => onClose()} text="Cancel" />
			</div>
		</Modal>
	);
};

export default ToggleUserForPost;
