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
import PostFilters from "../components/PostFilters";
import "../styles/pages/Home.css";

const Home = () => {
	const appContext = useContext(AppContext);
	const [filters, setFilters] = useState({
		showOnlyMyPosts: false,
		showHiddenPosts: false,
	});
	const [filteredPosts, setFilteredPosts] = useState(appContext.posts);
	const [isLoading, setIsLoading] = useState(true);
	const { user } = useAuth();
	const { lastMessageRegistered } = useSignalR();

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token)
			.then((postsFromServer) => {
				appContext.dispatch({ type: "LOAD_POSTS", posts: postsFromServer });
				setFilteredPosts(HelperFunctions.applyFilters(postsFromServer, filters, user.userId));
			})
			.finally(() => setIsLoading(false));
	};

	useEffect(() => {
		getPosts();
	}, [lastMessageRegistered]);

	useEffect(() => {
		console.log(new Date().toISOString());
		setFilteredPosts([...HelperFunctions.applyFilters(appContext.posts, filters, user.userId)]);
	}, [filters, appContext.posts]);

	return (
		<div className="home page">
			<Nav />
			<div className="container">
				<h1 className="container__title">Posts By Marko</h1>
				<p className="container__description">Create and share posts with your friends</p>
				{isLoading ? (
					<InfoMessage message="Loading Posts..." shouldAnimate />
				) : (
					<>
						{appContext.posts?.length > 0 ? (
							<div className="posts__dashboard">
								<PostFilters
									onFilterToggle={(isApplied, filter) =>
										setFilters({
											...filters,
											[filter]: isApplied,
										})
									}
								/>
								<ul className="posts__list">
									{filteredPosts.map((p) => (
										<Post
											key={p.postId}
											postId={p.postId}
											authorId={p.userId}
											title={p.title}
											content={p.content}
											isHidden={p.isHidden}
										/>
									))}
								</ul>
							</div>
						) : (
							<InfoMessage message="Seems there are no posts" />
						)}
						<UpdatePostForm />
						<DeletePostForm />
					</>
				)}
			</div>
		</div>
	);
};

export default Home;
