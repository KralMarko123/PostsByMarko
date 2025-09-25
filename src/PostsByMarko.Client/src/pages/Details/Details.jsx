import { React, useContext, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { HelperFunctions } from "../../util/helperFunctions";
import PostsService from "../../api/PostsService";
import Nav from "../../components/Layout/Nav/Nav";
import AppContext from "../../context/AppContext";
import Container from "../../components/Layout/Container/Container";
import logo from "../../assets/images/POSM_icon.png";
import { ICONS } from "../../constants/icons";
import Button from "../../components/Helper/Button/Button";
import TextareaAutosize from "react-textarea-autosize";
import { useSignalR } from "../../custom/useSignalR";
import "../Page.css";
import "./Details.css";

const Details = () => {
  const params = useParams();
  const postId = params.id;
  const { user } = useAuth();
  const appContext = useContext(AppContext);
  const [postDetails, setPostDetails] = useState({});
  const [errorMessage, setErrorMessage] = useState(null);
  const [confirmationalMessage, setConfirmationalMessage] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [updatedContent, setUpdatedContent] = useState("");
  const textAreaRef = useRef();
  const { sendMessage } = useSignalR();
  const postDetailsDate = HelperFunctions.getPostDetailsDate(
    postDetails.post?.createdDate
  );

  const getPost = async () => {
    await PostsService.getPostById(postId, user.token).then((requestResult) => {
      if (requestResult.statusCode === 200) {
        setErrorMessage(null);
        setPostDetails(requestResult.payload);
        setUpdatedContent(requestResult.payload.post.content);
      } else setErrorMessage(requestResult.message);
    });
  };

  const toggleEdit = (flag) => {
    setIsEditing(flag);
    textAreaRef.current.style.display = flag ? "block" : "none";

    if (!flag) {
      textAreaRef.current.value = postDetails.post.content;
      setUpdatedContent(postDetails.post.content);
      setErrorMessage(null);
    }
  };

  const handleUpdatedPostContent = async () => {
    if (updatedContent.length === 0) {
      setErrorMessage(`Content can't be empty`);
      return;
    }

    if (updatedContent === postDetails.post.content) {
      setErrorMessage(`You haven't made any changes`);
      return;
    }

    updatePostContent();
  };

  const updatePostContent = async () => {
    let updatedPost = postDetails.post;
    updatedPost.content = updatedContent;

    setErrorMessage(null);
    setIsLoading(true);

    await PostsService.updatePost(updatedPost, user.token)
      .then((requestResult) => {
        if (requestResult.statusCode === 200) {
          sendMessage("Updated Post");
          setConfirmationalMessage(requestResult.message);
          toggleEdit(false);

          setTimeout(() => {
            setConfirmationalMessage(null);
          }, 3000);
        } else setErrorMessage(requestResult.message);
      })
      .finally(() => setIsLoading(false));
  };

  useEffect(() => {
    getPost();

    appContext.dispatch({
      type: "MODIFYING_POST",
      post: { postId: postId },
    });
  }, []);

  return (
    <div className="details page">
      <img src={logo} className="logo" alt="posm-logo" />
      <Nav />

      <Container>
        <p>marko</p>
        {postDetails.post && (
          <>
            <div className="details-header">
              <h1 className="details-title">{postDetails.post?.title}</h1>
              <div className="author-container">
                {ICONS.USER_CIRCLE_ICON()}
                <p className="author">
                  By {postDetails.authorFirstName} {postDetails.authorLastName}
                </p>
                {ICONS.CLOCK_ICON()}
                <p className="date">{postDetailsDate}</p>
              </div>
            </div>

            <div className="details-container">
              <p className={`content ${isEditing ? "disabled" : ""}`}>
                {postDetails.post?.content}
              </p>
              <TextareaAutosize
                defaultValue={postDetails.post?.content}
                minRows={3}
                maxRows={20}
                ref={textAreaRef}
                onChange={() => setUpdatedContent(textAreaRef.current.value)}
              />
              <div className={`details-update-controls`}>
                {isEditing ? (
                  <>
                    <Button
                      additionalClassNames={"update-control"}
                      text={"Save"}
                      onButtonClick={() => handleUpdatedPostContent()}
                      loading={isLoading}
                    />
                    <Button
                      additionalClassNames={"update-control"}
                      text={"Cancel"}
                      onButtonClick={() => toggleEdit(false)}
                    />
                  </>
                ) : (
                  <Button
                    additionalClassNames={"update-control"}
                    text={"Edit"}
                    onButtonClick={() => toggleEdit(true)}
                  />
                )}
              </div>
            </div>
          </>
        )}

        {errorMessage && <p className="error">{errorMessage}</p>}
        {confirmationalMessage && (
          <p className="success fade-out">{confirmationalMessage}</p>
        )}
      </Container>
    </div>
  );
};

export default Details;
