import { React, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import { HelperFunctions } from "../util/helperFunctions";
import PostsService from "../api/PostsService";
import Nav from "../components/Layout/Nav";
import "../styles/pages/Details.css";
import Button from "../components/Helper/Button";

const Details = () => {
	const params = useParams();
	const postId = params.id;
	const [postDetails, setPostDetails] = useState({});
	const [isLoading, setIsLoading] = useState(true);
	const navigate = useNavigate();
	const { user } = useAuth();

	useEffect(() => {
		const getPost = async () => {
			await PostsService.getPostById(postId, user.token)
				.then((response) => setPostDetails(response))
				.catch((error) =>
					setPostDetails({
						post: {
							...postDetails.post,
							title: "Cannot open post",
							content: error.message,
						},
						authorFirstName: "Someone",
						authorLastName: "Hypothetical",
					})
				)
				.finally(() => setIsLoading(false));
		};
		getPost();
	}, []);

	return (
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
								<div className="footer__actions">
									
								</div>
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
		</div>
	);
};

export default Details;
