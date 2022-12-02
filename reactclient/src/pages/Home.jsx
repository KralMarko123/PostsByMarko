import { React, useState, useEffect } from "react";
import { useAuth } from "../custom/useAuth";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
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
			.build();

		setConnection(newConnection);
		getPosts();
	}, []);

	useEffect(() => {
		if (connection) {
			connection
				.start()
				.then((result) => {
					console.log("connected!");

					connection.on("ReceiveMessage", (message) => console.log(message));
				})
				.catch((error) => console.error(`Connection failed with error: ${error}`));
		}
	}, [connection]);

	const sendUpdatedPostMessage = async () => {
		if (connection.connectionStarted) {
			try {
				await connection.send("SendMessageToAll", "A Post has been updated!");
			} catch (e) {
				console.log(e);
			}
		} else {
			alert("No connection to server yet.");
		}
	};

	const onPostDeleted = (postId) => {
		setPosts(
			posts.filter((p) => {
				return p.postId !== postId;
			})
		);
	};

	const onPostUpdated = (updatedPost) => {
		const postIndex = posts.findIndex((p) => p.postId === updatedPost.postId);
		let updatedPosts = [...posts];
		updatedPosts[postIndex] = updatedPost;
		setPosts(updatedPosts);

		sendUpdatedPostMessage();
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
