import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Post from "../../components/Post/Post";
import Nav from "../../components/Layout/Nav/Nav";
import DeletePostForm from "../../components/Forms/DeletePostForm/DeletePostForm";
import UpdatePostForm from "../../components/Forms/UpdatePostForm/UpdatePostForm";
import AppContext from "../../context/AppContext";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
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
	}, [lastMessageRegistered, appContext.posts.length]);

	return (
		<div className="home page">
			<img src={logo} className="logo" alt="posm-logo" />
			<Nav />

			<Container
				title="Today's Posts"
				desc="Check out what's going on with the world. Create, edit & inspire"
			>
				<ul className="posts-list">
					{posts?.map((p, i) => (
						<Post
							key={p.id}
							id={p.id}
							authorId={p.userId}
							title={p.title}
							content={p.content}
							isHidden={p.isHidden}
							allowedUsers={p.allowedUsers}
							createdDate={p.createdDate}
							lastUpdatedDate={p.lastUpdatedDate}
							index={i}
						/>
					))}
				</ul>

				<UpdatePostForm />
				<DeletePostForm />
			</Container>
		</div>
	);
};

export default Home;
