import { React, useState, useEffect } from "react";
import { useAuth } from "../custom/useAuth";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import ENDPOINT__URLS from "../constants/endpoints";
import PostsService from "../api/PostsService";
import Button from "../components/Helper/Button";
import CreatePostForm from "../components/Forms/CreatePostForm";
import Post from "../components/Post";
import InfoMessage from "../components/Helper/InfoMessage";
import Nav from "../components/Layout/Nav";
import "../styles/pages/Home.css";

const Home = () => {
	const [connection, setConnection] = useState(null);
	const [posts, setPosts] = useState([]);
	const [isLoading, setIsLoading] = useState(true);
	const [showCreateForm, setShowCreateForm] = useState(false);
	const { user } = useAuth();

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token)
			.then((postsFromServer) => setPosts(postsFromServer))
			.catch((error) => console.error(error))
			.then(() => setIsLoading(false));
	};

	useEffect(() => {
		const newConnection = new HubConnectionBuilder()
			.withUrl(ENDPOINT__URLS.HUB, {
				accessTokenFactory: () => user.token,
			})
			.withAutomaticReconnect()
			.configureLogging(LogLevel.Information)
			.build();

		setConnection(newConnection);
		getPosts();
	}, []);

	useEffect(() => {
		if (connection) {
			connection
				.start()
				.then(() => {
					connection.on("ReceiveMessage", (message) => {
						switch (message) {
							case "Modified Post":
								getPosts();
								break;

							default:
								break;
						}
					});
				})
				.catch((error) => console.error(`Connection failed with error: ${error}`));
		}
	}, [connection]);

	const sendModifiedPost = async () => {
		if (connection) {
			try {
				await connection.send("SendMessageToAll", "Modified Post");
			} catch (error) {
				console.error(error);
			}
		} else console.log("A connection to the server hasn't been established yet!");
	};

	const onPostDeleted = (postId) => {
		setPosts(
			posts.filter((p) => {
				return p.postId !== postId;
			})
		);

		sendModifiedPost();
	};

	const onPostUpdated = (updatedPost) => {
		const postIndex = posts.findIndex((p) => p.postId === updatedPost.postId);
		let updatedPosts = [...posts];
		updatedPosts[postIndex] = updatedPost;
		setPosts(updatedPosts);

		sendModifiedPost();
	};

	return (
		<div className="home page">
			<Nav />
			<CreatePostForm
				isShown={showCreateForm}
				onSubmit={() => getPosts()}
				onClose={() => setShowCreateForm(false)}
			/>
			<div className="container">
				<h1 className="container__title">Welcome to our blog!</h1>
				<p className="container__description">
					Feel free to check out our posts and add one yourself
				</p>
				{isLoading ? (
					<InfoMessage message={"Loading Posts..."} shouldAnimate />
				) : (
					<ul className="posts__list">
						{posts.length > 0 ? (
							posts.map((p) => (
								<Post
									key={p.postId}
									postId={p.postId}
									authorId={p.userId}
									title={p.title}
									content={p.content}
									onPostDeleted={(postId) => onPostDeleted(postId)}
									onPostUpdated={(updatedPost) => onPostUpdated(updatedPost)}
								/>
							))
						) : (
							<InfoMessage message={"Seems there are no posts"} />
						)}
					</ul>
				)}
				<Button onButtonClick={() => setShowCreateForm(true)} text="Create Post" />
			</div>
		</div>
	);
};

export default Home;
