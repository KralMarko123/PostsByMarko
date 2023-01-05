import { React, createContext } from "react";

const AppContext = createContext({
	posts: [],
	modalVisibility: {
		createPost: false,
		updatePost: false,
		deletePost: false,
		addUserToPost: false,
	},
	postBeingModified: {},
});

export default AppContext;
