import { React, useReducer } from "react";
import AppContext from "./AppContext";

export const defaultAppState = {
	posts: [],
	modalVisibility: {
		createPost: false,
		updatePost: false,
		deletePost: false,
		addUserToPost: false,
	},
	postBeingModified: {
		postId: null,
		title: "",
		content: "",
		index: null,
	},
};

export const appReducer = (state, action) => {
	let posts;
	let postBeingModifiedIndex;

	switch (action.type) {
		case "SHOW_MODAL":
			return { ...state, modalVisibility: { ...state.modalVisibility, [action.modal]: true } };

		case "CLOSE_MODAL":
			return { ...state, modalVisibility: { ...state.modalVisibility, [action.modal]: false } };

		case "LOAD_POSTS":
			return { ...state, posts: action.posts };

		case "MODIFYING_POST":
			return { ...state, postBeingModified: action.post };

		case "DELETED_POST":
			return { ...state, posts: [...state.posts.filter((p) => p.postId !== action.postId)] };

		case "UPDATED_POST":
			postBeingModifiedIndex = [...state.posts].findIndex((p) => p.postId === action.post.postId);
			posts = [...state.posts];

			posts[postBeingModifiedIndex].title = action.post.title;
			posts[postBeingModifiedIndex].content = action.post.content;
			posts[postBeingModifiedIndex].lastUpdatedDate = new Date().toISOString();

			return { ...state, posts: posts };

		case "CREATED_POST":
			posts = [...state.posts];

			posts.push(action.post);

			return { ...state, posts: posts };

		case "TOGGLE_POST_HIDDEN":
			postBeingModifiedIndex = [...state.posts].findIndex((p) => p.postId === action.postId);
			posts = [...state.posts];

			posts[postBeingModifiedIndex].isHidden = !posts[postBeingModifiedIndex].isHidden;
			posts[postBeingModifiedIndex].lastUpdatedDate = new Date().toISOString();

			return { ...state, posts: posts };

		case "TOGGLED_USER":
			postBeingModifiedIndex = [...state.posts].findIndex((p) => p.postId == action.postId);
			posts = [...state.posts];

			if (action.isAdded) posts[postBeingModifiedIndex].allowedUsers.push(action.username);
			else
				posts[postBeingModifiedIndex].allowedUsers = posts[
					postBeingModifiedIndex
				].allowedUsers.filter((u) => u !== action.username);

			return { ...state, posts: posts };

		default:
			return defaultAppState;
	}
};

const AppProvider = ({ children }) => {
	const [appState, dispatch] = useReducer(appReducer, defaultAppState);

	const appContext = {
		posts: appState.posts,
		modalVisibility: appState.modalVisibility,
		postBeingModified: appState.postBeingModified,

		dispatch: dispatch,
	};

	return <AppContext.Provider value={appContext}>{children}</AppContext.Provider>;
};

export default AppProvider;
