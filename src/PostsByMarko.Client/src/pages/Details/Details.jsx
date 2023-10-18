import { React, useContext, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { HelperFunctions } from "../../util/helperFunctions";
import PostsService from "../../api/PostsService";
import Nav from "../../components/Layout/Nav";
import AppContext from "../../context/AppContext";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
import { ICONS } from "../../constants/icons";
import Button from "../../components/Helper/Button/Button";
import "../Page.css";
import "./Details.css";

const Details = () => {
	const navigate = useNavigate();
	const params = useParams();
	const postId = params.id;
	const [postDetails, setPostDetails] = useState({});
	const [errorMessage, setErrorMessage] = useState("");
	const { user } = useAuth();
	const appContext = useContext(AppContext);
	const postDetailsDate = HelperFunctions.getPostDetailsDate(postDetails.post?.createdDate);
	const [isEditing, setIsEditing] = useState(false);

	const getPost = async () => {
		await PostsService.getPostById(postId, user.token).then((requestResult) => {
			if (requestResult.statusCode === 200) {
				setErrorMessage("");
				setPostDetails(requestResult.payload);
			} else setErrorMessage(requestResult.message);
		});
	};

	const handleTextAreaInput = (e) => {
		e.target.style.height = "inherit";

		const computed = window.getComputedStyle(e.target);
		const height = parseInt(computed.getPropertyValue("padding"), 10) + e.target.scrollHeight;

		e.target.style.height = `${height}px`;
	};

	useEffect(() => {
		getPost();
		appContext.dispatch({
			type: "MODIFYING_POST",
			post: { postId: postId },
		});
	}, []);

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
							<p className={`content ${isEditing ? "disabled" : ""}`}>
								{postDetails.post?.content}
							</p>
							<textarea
								className={`details-update ${isEditing ? "open" : ""}`}
								defaultValue={postDetails.post?.content}
								onKeyDown={(e) => handleTextAreaInput(e)}
							/>
						</div>
						<div className={`details-update-controls`}>
							{isEditing ? (
								<>
									<Button additionalClassNames={"update-control"} text={"Save"} />
									<Button
										additionalClassNames={"update-control"}
										text={"Cancel"}
										onButtonClick={() => setIsEditing(false)}
									/>
								</>
							) : (
								<Button
									additionalClassNames={"update-control"}
									text={"Edit"}
									onButtonClick={() => setIsEditing(true)}
								/>
							)}
						</div>
					</>
				)}

				{errorMessage && <p className="error">{errorMessage}</p>}
			</Container>
		</div>
	);
};

export default Details;
