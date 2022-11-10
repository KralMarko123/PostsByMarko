import { React, useState, useEffect } from "react";
import PostsService from "../api/PostsService";
import Button from "../components/UI/Button";
import CreatePostForm from "../components/Forms/CreatePostForm";
import Post from "../components/Post";
import "../styles/pages/AllPosts.css";

const AllPosts = () => {
	const [posts, setPosts] = useState([]);
	const [isLoading, setIsLoading] = useState(true);
	const [showCreateForm, setShowCreateForm] = useState(false);

	const getPosts = async () => {
		await PostsService.getAllPosts()
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
		<div className="all-posts page">
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
					<p className="info__message">Loading Posts...</p>
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
							<p className="info__message no__animation">Seems there are no posts.</p>
						)}
					</ul>
				)}
				<Button onButtonClick={() => setShowCreateForm(true)} text="Create Post" />
			</div>
		</div>
	);
};

export default AllPosts;
