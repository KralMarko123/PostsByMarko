import { React, useReducer } from "react";
import AppContext from "./AppContext";

export const defaultAppState = {
	posts: [],
	modalVisibility: {
		createPost: false,
		updatePost: false,
		deletePost: false,
	},
	postBeingModified: {},
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

		case "DELETE_POST":
			postBeingModifiedIndex = [...state.posts].findIndex((p) => p.postId === action.postId);
			posts = [...state.posts.splice(postBeingModifiedIndex, 1)];

			return { ...state, posts: [...state.posts.splice(postBeingModifiedIndex, 1)] };

		case "TOGGLE_POST_HIDDEN":
			posts = [...state.posts];
			posts[action.postPosition].isHidden = !posts[action.postPosition].isHidden;

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
