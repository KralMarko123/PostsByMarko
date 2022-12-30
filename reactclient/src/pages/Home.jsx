import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../custom/useAuth";
import { useSignalR } from "../custom/useSignalR";
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
				setFilteredPosts(postsFromServer);
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
					setFilteredPosts([...filteredPosts.filter((p) => p.userId === user.userId)]);
					break;
				case "byLastUpdatedDate":
					setFilteredPosts([
						...filteredPosts.sort((p1, p2) => {
							const date1 = Date.parse(p1.lastUpdatedDate);
							const date2 = Date.parse(p2.lastUpdatedDate);

							return date1 > date2 ? -1 : 1;
						}),
					]);

				default:
					break;
			}
		} else setFilteredPosts(appContext.posts);
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
								<span className="filter">
									By Date
									<input
										type="checkbox"
										name="byLastUpdatedDate"
										id="byLastUpdatedDate"
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
