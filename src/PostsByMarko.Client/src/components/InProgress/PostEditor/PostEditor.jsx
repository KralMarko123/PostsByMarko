import React, { useState, useEffect } from "react";
import { EditorState } from "draft-js";
import { Editor } from "react-draft-wysiwyg";
import { convertToHTML } from "draft-convert";
import { HelperFunctions } from "../../../util/helperFunctions";
import "react-draft-wysiwyg/dist/react-draft-wysiwyg.css";
import "./PostEditor.css";

const PostEditor = () => {
	const [editorState, setEditorState] = useState(() => EditorState.createEmpty());
	const [convertedContent, setConvertedContent] = useState(null);

	useEffect(() => {
		const cleanHtml = HelperFunctions.getPurifiedHtml(
			convertToHTML(editorState.getCurrentContent())
		);

		setConvertedContent(cleanHtml);
	}, [editorState]);

	return (
		<>
			<Editor
				editorState={editorState}
				onEditorStateChange={setEditorState}
				wrapperClassName="post-editor"
				toolbarClassName="toolbar"
				editorClassName="editor"
			/>

			<div className="editor-output" dangerouslySetInnerHTML={convertedContent}></div>
		</>
	);
};

export default PostEditor;
