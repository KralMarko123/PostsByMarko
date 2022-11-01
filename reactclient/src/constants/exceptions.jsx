export class POST_NOT_FOUND_ERROR extends Error {
	constructor(message) {
		super(message);
		this.name = "POST_NOT_FOUND";
	}
}

export class CREATE_POST_ERROR extends Error {
	constructor(message) {
		super(message);
		this.name = "CREATE_POST_ERROR";
	}
}

export class UPDATE_POST_ERROR extends Error {
	constructor(message) {
		super(message);
		this.name = "UPDATE_POST_ERROR";
	}
}

export class DELETE_POST_ERROR extends Error {
	constructor(message) {
		super(message);
		this.name = "DELETE_POST_ERROR";
	}
}
