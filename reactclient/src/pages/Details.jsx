import { React, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import PostsService from "../api/PostsService";
import Nav from "../components/Layout/Nav";
import "../styles/pages/Details.css";
import { HelperFunctions } from "../util/helperFunctions";

const Details = () => {
	const params = useParams();
	const postId = params.id;
	const [postDetails, setPostDetails] = useState({
		post: {
			postId: "",
			title: "",
			content: "",
			userId: "",
			createdDate: "",
			lastUpdatedDate: "",
		},
		author: {
			username: "",
			firstName: "",
			lastName: "",
			roles: [],
		},
	});
	const [isLoading, setIsLoading] = useState(true);
	const navigate = useNavigate();
	const { user } = useAuth();

	useEffect(() => {
		const getPost = async () => {
			await PostsService.getPostById(postId, user.token)
				.then((postFromServer) => {
					setPostDetails(postFromServer);
				})
				.catch((error) => {
					console.error(error);
					setPostDetails({
						post: {
							...postDetails.post,
							title: "No Post Found",
							content: `The post with Id: ${postId} doesn't seem to exist. Go back to view other posts`,
						},
						author: { ...postDetails.author, firstName: "Yours", lastName: "Truly" },
					});
				})
				.then(() => setIsLoading(false));
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
						<div className="container__footer">
							<p className="footer__author">
								BY {`${postDetails.author.firstName} ${postDetails.author.lastName}`}
							</p>
							<span className="footer__date">
								Created on {HelperFunctions.getDateAsReadablestring(postDetails.post.createdDate)}
							</span>
						</div>
					</>
				)}
			</div>
		</div>
	);
};

export default Details;
