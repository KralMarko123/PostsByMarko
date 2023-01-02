import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../custom/useAuth";
import { useSignalR } from "../custom/useSignalR";
import { HelperFunctions } from "../util/helperFunctions";
import PostsService from "../api/PostsService";
import Post from "../components/Post";
import InfoMessage from "../components/Helper/InfoMessage";
import Nav from "../components/Layout/Nav";
import UpdatePostForm from "../components/Forms/UpdatePostForm";
import DeletePostForm from "../components/Forms/DeletePostForm";
import AppContext from "../context/AppContext";
import "../styles/pages/Home.css";

const Home = () => {
	const appContext = useContext(AppContext);
	const [filterToggled, setFilterToggled] = useState({
		isFilterApplied: false,
		filterType: "",
	});
	const [filteredPosts, setFilteredPosts] = useState(appContext.posts);
	const [isLoading, setIsLoading] = useState(true);
	const { user } = useAuth();
	const { lastMessageRegistered } = useSignalR();

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token)
			.then((postsFromServer) => {
				appContext.dispatch({ type: "LOAD_POSTS", posts: postsFromServer });
				setFilteredPosts(HelperFunctions.sortPostsByLastUpdatedDate(postsFromServer));
			})
			.catch((error) => console.error(error))
			.then(() => setIsLoading(false));
	};

	const handleFilterToggle = (e) => {
		setFilterToggled({
			isFilterApplied: e.target.checked,
			filterType: e.target.name,
		});
	};

	useEffect(() => {
		getPosts();
	}, [lastMessageRegistered]);

	useEffect(() => {
		if (filterToggled.isFilterApplied) {
			switch (filterToggled.filterType) {
				case "myPosts":
					setFilteredPosts(HelperFunctions.filterPostsByUserId(filteredPosts, user));
					break;

				default:
					break;
			}
		} else setFilteredPosts(HelperFunctions.sortPostsByLastUpdatedDate(appContext.posts));
	}, [filterToggled.isFilterApplied]);

	return (
		<div className="home page">
			<Nav />
			<div className="container">
				<h1 className="container__title">Posts By Marko</h1>
				<p className="container__description">Create and share posts with your friends</p>
				{isLoading ? (
					<InfoMessage message={"Loading Posts..."} shouldAnimate />
				) : (
					<>
						<div className="posts__dashboard">
							<div className="dashboard__filters">
								<p>Filters:</p>
								<span className="filter">
									My Posts
									<input
										type="checkbox"
										name="myPosts"
										id="myPosts"
										onChange={(e) => handleFilterToggle(e)}
									/>
								</span>
							</div>
							<ul className="posts__list">
								{filteredPosts.map((p) => (
									<Post
										key={p.postId}
										postId={p.postId}
										authorId={p.userId}
										title={p.title}
										content={p.content}
									/>
								))}
							</ul>
						</div>
						<UpdatePostForm />
						<DeletePostForm />
					</>
				)}
			</div>
		</div>
	);
};

export default Home;
