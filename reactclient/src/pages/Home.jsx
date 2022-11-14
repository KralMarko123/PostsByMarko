import { React, useState, useEffect } from "react";
import { useAuth } from "../custom/useAuth";
import PostsService from "../api/PostsService";
import Button from "../components/UI/Button";
import CreatePostForm from "../components/Forms/CreatePostForm";
import Post from "../components/Post";
import InfoMessage from "../components/UI/InfoMessage";
import Nav from "../components/Layout/Nav";
import "../styles/pages/Home.css";

const Home = () => {
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
		getPosts();
	}, []);

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
					Feel free to check out our posts and add one yourself.
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
									title={p.title}
									content={p.content}
									onPostDeleted={(postId) => onPostDeleted(postId)}
									onPostUpdated={(updatedPost) => onPostUpdated(updatedPost)}
								/>
							))
						) : (
							<InfoMessage message={"Seems there are no posts."} />
						)}
					</ul>
				)}
				<Button onButtonClick={() => setShowCreateForm(true)} text="Create Post" />
			</div>
		</div>
	);
};

export default Home;
