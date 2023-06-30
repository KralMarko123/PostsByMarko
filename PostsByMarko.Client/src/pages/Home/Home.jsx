import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import { HelperFunctions } from "../../util/helperFunctions";
import PostsService from "../../api/PostsService";
import Post from "../../components/Post";
import InfoMessage from "../../components/Helper/InfoMessage";
import Nav from "../../components/Layout/Nav";
import UpdatePostForm from "../../components/Forms/UpdatePostForm";
import DeletePostForm from "../../components/Forms/DeletePostForm";
import AppContext from "../../context/AppContext";
import PostFilters from "../../components/PostFilters";
import Refetcher from "../../components/Helper/Refetcher";
import "../Page.css";
import "./Home.css";

const Home = () => {
	const appContext = useContext(AppContext);
	const { user } = useAuth();
	const { lastMessageRegistered } = useSignalR();
	const [filters, setFilters] = useState({
		showOnlyMyPosts: false,
		showHiddenPosts: false,
	});
	const [filteredPosts, setFilteredPosts] = useState(
		HelperFunctions.applyFilters(appContext.posts, filters, user.userId)
	);

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token).then((postsFromServer) => {
			appContext.dispatch({ type: "LOAD_POSTS", posts: postsFromServer });
			setFilteredPosts(HelperFunctions.applyFilters(postsFromServer, filters, user.userId));
		});
	};

	useEffect(() => {
		getPosts();
	}, [lastMessageRegistered]);

	useEffect(() => {
		setFilteredPosts([...HelperFunctions.applyFilters(appContext.posts, filters, user.userId)]);
	}, [filters, appContext.posts]);

	return (
		<Refetcher>
			<div className="home page">
				<Nav />
				<div className="container">
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
								{filteredPosts?.map((p, i) => (
									<Post
										key={p.postId}
										postId={p.postId}
										authorId={p.userId}
										title={p.title}
										content={p.content}
										isHidden={p.isHidden}
										allowedUsers={p.allowedUsers}
									/>
								))}
							</ul>
						</div>
					) : (
						<InfoMessage message="Seems there are no posts" />
					)}
					<UpdatePostForm />
					<DeletePostForm />
				</div>
			</div>
		</Refetcher>
	);
};

export default Home;
