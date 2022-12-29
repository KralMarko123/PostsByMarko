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
	switch (action.type) {
		case "SHOW_MODAL":
			return { ...state, modalVisibility: { ...state.modalVisibility, [action.modal]: true } };
		case "CLOSE_MODAL":
			return { ...state, modalVisibility: { ...state.modalVisibility, [action.modal]: false } };
		case "LOAD_POSTS":
			return { ...state, posts: action.posts };
		case "MODIFYING_POST":
			return { ...state, postBeingModified: action.post };

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
