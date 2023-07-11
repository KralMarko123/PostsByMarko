import { React, useContext, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ROUTES } from "../../constants/routes";
import { useAuth } from "../../custom/useAuth";
import { HelperFunctions } from "../../util/helperFunctions";
import PostsService from "../../api/PostsService";
import Nav from "../../components/Layout/Nav";
import ToggleUserForPost from "../../components/Forms/ToggleUserForPost";
import AppContext from "../../context/AppContext";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
import { ICONS } from "../../constants/icons";
import "../Page.css";
import "./Details.css";

const Details = () => {
	const navigate = useNavigate();
	const params = useParams();
	const postId = params.id;
	const [postDetails, setPostDetails] = useState({});
	const [errorMessage, setErrorMessage] = useState("");
	const [isLoading, setIsLoading] = useState(true);
	const { user, isAdmin } = useAuth();
	const appContext = useContext(AppContext);
	const postDetailsDate = HelperFunctions.getPostDetailsDate(postDetails.post?.createdDate);

	const getPost = async () => {
		await PostsService.getPostById(postId, user.token)
			.then((requestResult) => {
				if (requestResult.statusCode === 200) {
					setErrorMessage("");
					setPostDetails(requestResult.payload);
				} else setErrorMessage(requestResult.message);
			})
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
		<div className="details page">
			<img src={logo} className="logo" alt="posm-logo" />
			<Nav />

			<Container>
				{postDetails.post && (
					<>
						<div className="details-header">
							<h1 className="details-title">{postDetails.post?.title}</h1>
							<div className="author-container">
								{ICONS.USER_CIRCLE_ICON()}
								<p className="author">
									By {postDetails.authorFirstName} {postDetails.authorLastName}
								</p>
								{ICONS.CLOCK_ICON()}
								<p className="date">{postDetailsDate}</p>
							</div>
						</div>
						<div className="details-container">
							<p className="content">{postDetails.post?.content}</p>
						</div>
					</>
				)}

				{errorMessage && <p className="error">{errorMessage}</p>}
			</Container>

			<ToggleUserForPost />
		</div>
	);
};

export default Details;
