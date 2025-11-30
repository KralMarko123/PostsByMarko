import { React, useContext, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { PostService } from "../../api/PostService";
import { ICONS } from "../../constants/icons";
import { ROUTES } from "../../constants/routes";
import { DateFunctions } from "../../util/dateFunctions";
import Nav from "../../components/Layout/Nav/Nav";
import Container from "../../components/Layout/Container/Container";
import Button from "../../components/Helper/Button/Button";
import TextareaAutosize from "react-textarea-autosize";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import "../Page.css";
import "./Details.css";

const Details = () => {
  const params = useParams();
  const postId = params.id;
  const { user, isAdmin } = useAuth();
  const [post, setPost] = useState({});
  const [errorMessage, setErrorMessage] = useState(null);
  const [confirmationalMessage, setConfirmationalMessage] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [updatedContent, setUpdatedContent] = useState("");
  const textAreaRef = useRef();
  const navigate = useNavigate();
  const postCreatedDate = DateFunctions.getLocalDateInFormat(
    post?.createdDate,
    "DD MMMM YYYY"
  );
  const [author, setAuthor] = useState({});
  const isAuthor = author.id === user.id;

  const getPost = async () => {
    await PostService.getPostById(postId, user.token)
      .then((postResponse) => {
        setErrorMessage(null);
        setPost(postResponse);
        setAuthor(postResponse.author);
        setUpdatedContent((prev) => postResponse.content);
      })
      .catch((error) => setErrorMessage(error.message));
  };

  const toggleEdit = (flag) => {
    textAreaRef.current.style.display = flag ? "block" : "none";

    if (flag) {
      textAreaRef.current.value = post.content;
    }

    setIsEditing(flag);
    setErrorMessage(null);
  };

  const handleUpdatedPostContent = async () => {
    if (updatedContent.length === 0) {
      setErrorMessage(`Content can't be empty`);
      return;
    }

    if (updatedContent === post.content) {
      setErrorMessage(`You haven't made any changes`);
      return;
    }

    await updatePostContent();
  };

  const updatePostContent = async () => {
    let updatePostRequest = {
      title: post.title,
      hidden: post.hidden,
      content: updatedContent,
    };

    setErrorMessage(null);
    setIsLoading(true);

    await PostService.updatePost(post.id, updatePostRequest, user.token)
      .then((updatedPostResponse) => {
        setPost(updatedPostResponse);
        setAuthor(updatedPostResponse.author);
        setUpdatedContent(updatedPostResponse.content);
        setConfirmationalMessage("Successfully updated Post!");

        textAreaRef.current.value = updatedPostResponse.content;
        toggleEdit(false);

        setTimeout(() => {
          setConfirmationalMessage(null);
        }, 3000);
      })
      .catch((error) => setErrorMessage(error.message))
      .finally(() => setIsLoading(false));
  };

  useEffect(() => {
    getPost();
  }, []);

  return (
    <div className="details page">
      <Logo />
      <Nav />

      <Container>
        {post.title && (
          <>
            <div className="details-header">
              <h1 className="details-title">{post.title}</h1>
              <div className="author-container">
                <div className="box">
                  {ICONS.USER_CIRCLE_ICON()}
                  <p className="author">
                    By {author.firstName} {author.lastName}
                  </p>
                </div>
                <div className="box">
                  {ICONS.CLOCK_ICON()}
                  <p className="date">{postCreatedDate}</p>
                </div>
              </div>
            </div>

            <div className="details-container">
              <p className={`content ${isEditing ? "disabled" : ""}`}>
                {post.content}
              </p>
              <TextareaAutosize
                defaultValue={post.content}
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
                  (isAuthor || isAdmin) && (
                    <Button
                      additionalClassNames={"update-control"}
                      text={"Edit"}
                      onButtonClick={() => toggleEdit(true)}
                    />
                  )
                )}

                <Button
                  additionalClassNames={"update-control"}
                  text={"Back"}
                  onButtonClick={() => navigate(ROUTES.HOME)}
                />
              </div>
            </div>
          </>
        )}

        {errorMessage && <p className="error">{errorMessage}</p>}
        {confirmationalMessage && (
          <p className="success fade-out">{confirmationalMessage}</p>
        )}
      </Container>

      <Footer />
    </div>
  );
};

export default Details;
