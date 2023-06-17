import { React, useContext, useEffect, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import { modalTransitionDuration } from "../../constants/misc";
import Modal from "../Helper/Modal";
import AppContext from "../../context/AppContext";
import Button from "../Helper/Button";
import UsersService from "../../api/UsersService";
import PostsService from "../../api/PostsService";
import Select from "../Gen/Select";
import "../../styles/components/ToggleUserForPost.css";

const ToggleUserForPost = () => {
	const { user } = useAuth();
	const [users, setUsers] = useState([]);
	const [selectedUser, setSelectedUser] = useState(null);
	const [message, setMessage] = useState(null);
	const appContext = useContext(AppContext);
	const { sendMessage } = useSignalR();
	const [isLoading, setIsLoading] = useState(false);

	const getUsers = async () => {
		await UsersService.getAllUsers(user.token).then((usernames) => {
			let usersFromServer = [];

			usernames
				.filter((u) => u !== user.username)
				.forEach((u) => {
					usersFromServer.push({
						value: u,
						flag: isAllowedUser(u) ? "( Allowed )" : "( Hidden )",
					});
				});

			setUsers(usersFromServer);
		});
	};

	const isAllowedUser = (username) => {
		return appContext.posts
			.find((p) => p.postId == appContext.postBeingModified.postId)
			?.allowedUsers.includes(username);
	};

	const onClose = () => {
		appContext.dispatch({ type: "CLOSE_MODAL", modal: "addUserToPost" });

		setTimeout(() => {
			setMessage(null);
			setSelectedUser(null);
		}, modalTransitionDuration);
	};

	const onToggle = async () => {
		if (!selectedUser) {
			setMessage({ isSuccessful: false, message: "Please select a user" });
			return;
		} else {
			setIsLoading(true);
			await PostsService.toggleUserForPost(
				appContext.postBeingModified.postId,
				selectedUser,
				user.token
			)
				.then((response) => {
					appContext.dispatch({
						type: "TOGGLED_USER",
						username: selectedUser,
						isAdded: !isAllowedUser(selectedUser),
						postId: appContext.postBeingModified.postId,
					});

					sendMessage("Toggled User For Post");
					setMessage(response);

					setTimeout(() => {
						onClose();
					}, 1000);
				})
				.catch((error) =>
					setMessage({
						isSuccessful: false,
						message: error.message,
					})
				)
				.finally(() => setIsLoading(false));
		}
	};

	useEffect(() => {
		getUsers();
	}, [appContext.posts]);

	return (
		<Modal
			isShown={appContext.modalVisibility.addUserToPost}
			title="Toggle User For Post"
			onClose={() => onClose()}
			message={message}
		>
			<div className="allowed__users__container">
				<Select
					value={selectedUser}
					options={users}
					onChange={(option) => setSelectedUser(option?.value)}
				/>
				<Button onButtonClick={() => onToggle()} text="Submit" loading={isLoading} />
				<Button onButtonClick={() => onClose()} text="Cancel" />
			</div>
		</Modal>
	);
};

export default ToggleUserForPost;
