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
	const [isLoading, setIsLoading] = useState(true);
	const { user } = useAuth();
	const { lastMessageRegistered } = useSignalR();

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token)
			.then((postsFromServer) =>
				appContext.dispatch({ type: "LOAD_POSTS", posts: postsFromServer })
			)
			.catch((error) => console.error(error))
			.then(() => setIsLoading(false));
	};

	useEffect(() => {
		getPosts();
		console.log("here");
	}, [lastMessageRegistered]);

	return (
		<div className="home page">
			<Nav />
			<div className="container">
				<h1 className="container__title">Posts By Marko</h1>
				<p className="container__description">Create and share posts with your friends</p>
				{isLoading ? (
					<InfoMessage message={"Loading Posts..."} shouldAnimate />
				) : (
					<ul className="posts__list">
						{appContext.posts.length > 0 ? (
							<>
								{appContext.posts.map((p) => (
									<Post
										key={p.postId}
										postId={p.postId}
										authorId={p.userId}
										title={p.title}
										content={p.content}
									/>
								))}
								<UpdatePostForm />
								<DeletePostForm />
							</>
						) : (
							<InfoMessage message={"Seems there are no posts"} />
						)}
					</ul>
				)}
			</div>
		</div>
	);
};

export default Home;
