import { React, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../constants/routes";
import { useAuth } from "../custom/useAuth";
import PostsService from "../api/PostsService";
import Nav from "../components/Layout/Nav";
import "../styles/pages/Details.css";

const Details = () => {
	const params = useParams();
	const postId = params.id;
	const [postDetails, setPostDetails] = useState({
		title: "",
		content: "",
	});
	const [isLoading, setIsLoading] = useState(true);
	const navigate = useNavigate();
	const { user } = useAuth();

	useEffect(() => {
		const getPost = async () => {
			await PostsService.getPostById(postId, user.token)
				.then((postFromServer) => {
					setPostDetails({
						title: postFromServer.title,
						content: postFromServer.content,
					});
				})
				.catch((error) => {
					console.error(error);
					setPostDetails({
						title: "No Post Found",
						content: `The post with Id: ${postId} doesn't seem to exist. Go back to view other posts`,
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
						<h1 className="container__title">{postDetails.title}</h1>
						<p className="container__description">{postDetails.content}</p>{" "}
					</>
				)}
			</div>
		</div>
	);
};

export default Details;
