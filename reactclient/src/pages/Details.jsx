import { React, useContext, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import { HelperFunctions } from "../util/helperFunctions";
import PostsService from "../api/PostsService";
import Nav from "../components/Layout/Nav";
import ToggleUserForPost from "../components/Forms/ToggleUserForPost";
import AppContext from "../context/AppContext";
import Refetcher from "../components/Helper/Refetcher";
import "../styles/pages/Details.css";

const Details = () => {
	const params = useParams();
	const postId = params.id;
	const navigate = useNavigate();
	const [postDetails, setPostDetails] = useState({
		post: {
			allowedUsers: [],
		},
		authorFirstName: "",
		authorLastName: "",
		isAuthor: false,
	});
	const [isLoading, setIsLoading] = useState(true);
	const { user, isAdmin } = useAuth();
	const appContext = useContext(AppContext);

	const getPost = async () => {
		await PostsService.getPostById(postId, user.token)
			.then((response) => setPostDetails(response))
			.catch((error) =>
				setPostDetails({
					post: {
						title: "Cannot open post",
						content: error.message,
					},
					authorFirstName: "Someone",
					authorLastName: "Hypothetical",
				})
			)
			.finally(() => setIsLoading(false));
	};

	useEffect(() => {
		getPost();
		appContext.dispatch({
			type: "MODIFYING_POST",
			post: { postId: postId },
		});
	}, []);

	const openAddUsersModal = () => {
		appContext.dispatch({ type: "SHOW_MODAL", modal: "addUserToPost" });
	};

	return (
		<Refetcher>
			<div className="details page">
				<Nav />
				<div className="container">
					<span className="container__back" onClick={() => navigate(ROUTES.HOME)}>
						Back
					</span>
					{isLoading ? (
						<p className="info__message">Loading Post Details...</p>
					) : (
						<>
							<h1 className="container__title">{postDetails.post.title}</h1>
							<p className="container__description">{postDetails.post.content}</p>
							{postDetails.post && (
								<div className="container__footer">
									{(postDetails.isAuthor || isAdmin) && (
										<div className="footer__actions">
											<div className="footer__action" onClick={() => openAddUsersModal()}>
												Toggle Users
											</div>
										</div>
									)}
									<p className="footer__author">
										BY {`${postDetails.authorFirstName} ${postDetails.authorLastName}`}
									</p>
									<span className="footer__date">
										Created on {HelperFunctions.getDateAsReadableText(postDetails.post.createdDate)}
									</span>
								</div>
							)}
						</>
					)}
				</div>
				<ToggleUserForPost />
			</div>
		</Refetcher>
	);
};

export default Details;
