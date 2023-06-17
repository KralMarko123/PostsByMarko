import React, { useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import PostsService from "../../api/PostsService";
import AppContext from "../../context/AppContext";

const Refetcher = ({ children }) => {
	const appContext = useContext(AppContext);
	const { user } = useAuth();

	const getPosts = async () => {
		await PostsService.getAllPosts(user.token).then((postsFromServer) => {
			appContext.dispatch({ type: "LOAD_POSTS", posts: postsFromServer });
		});
	};

	useEffect(() => {
		if (appContext.posts.length === 0) getPosts();
	}, []);

	return <>{children}</>;
};

export default Refetcher;
