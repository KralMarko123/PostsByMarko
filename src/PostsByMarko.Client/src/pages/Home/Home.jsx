import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Post from "../../components/Post/Post";
import InfoMessage from "../../components/Helper/InfoMessage";
import Nav from "../../components/Layout/Nav";
import UpdatePostForm from "../../components/Forms/UpdatePostForm";
import DeletePostForm from "../../components/Forms/DeletePostForm";
import AppContext from "../../context/AppContext";
import Refetcher from "../../components/Helper/Refetcher";
import "../Page.css";
import "./Home.css";

const Home = () => {
	const appContext = useContext(AppContext);
	const { user } = useAuth();
	const { lastMessageRegistered } = useSignalR();
	const [posts, setPosts] = useState([]);

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token).then((requestResult) => {
			setPosts(requestResult.payload);
			appContext.dispatch({ type: "LOAD_POSTS", posts: requestResult.payload });
		});
	};

	useEffect(() => {
		getPosts();
	}, [lastMessageRegistered, appContext.posts]);

	return (
		<Refetcher>
			<div className="home page">
				<Nav />
				<div className="container">
					{appContext.posts?.length > 0 ? (
						<ul className="posts__list">
							{posts.map((p) => (
								<Post
									key={p.id}
									id={p.id}
									authorId={p.userId}
									title={p.title}
									content={p.content}
									isHidden={p.isHidden}
									allowedUsers={p.allowedUsers}
									createdDate={p.createdDate}
								/>
							))}
						</ul>
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
