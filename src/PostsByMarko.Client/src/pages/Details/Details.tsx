import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";
import { AppContext } from "../../context/AppContext";
import { useEffect, useRef, useState, useContext } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { PostService } from "../../api/PostService";
import { ICONS } from "../../constants/icons";
import { ROUTES } from "../../constants/routes";
import { DateFunctions } from "../../util/dateFunctions";
import { Container } from "../../components/Layout/Container/Container";
import { Button } from "../../components/Helper/Button/Button";
import TextareaAutosize from "react-textarea-autosize";
import { Post } from "@typeConfigs/post";
import { User } from "@typeConfigs/user";

export const Details = () => {
  const { lastMessageRegistered } = useContext(AppContext);
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();
  const params = useParams();
  const postId = params.id;
  const [post, setPost] = useState<Post>();
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [confirmationalMessage, setConfirmationalMessage] = useState<string>("");
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [updatedContent, setUpdatedContent] = useState<string>("");
  const textAreaRef = useRef<HTMLTextAreaElement>(null);

  const [author, setAuthor] = useState<User>();
  const isAuthor = author?.id === user?.id;

  const getPost = async () => {
    await PostService.getPostById(postId!, user!.token!)
      .then((postResponse) => {
        setErrorMessage("");
        setPost(postResponse);
        setAuthor(postResponse.author!);
        setUpdatedContent(postResponse.content);
      })
      .catch((error) => setErrorMessage(error.message));
  };

  const toggleEdit = (flag: boolean) => {
    if (flag) setUpdatedContent(post!.content);

    setIsEditing(flag);
    setErrorMessage("");
  };

  const handleUpdatedPostContent = async () => {
    if (updatedContent.length === 0) {
      setErrorMessage(`Content can't be empty`);
      return;
    }

    if (updatedContent === post!.content) {
      setErrorMessage(`You haven't made any changes`);
      return;
    }

    await updatePostContent();
  };

  const updatePostContent = async () => {
    const updatePostRequest = {
      title: post!.title,
      hidden: post!.hidden,
      content: updatedContent,
    };

    setErrorMessage("");
    setIsLoading(true);

    await PostService.updatePost(post!.id!, updatePostRequest, user!.token!)
      .then((updatedPostResponse) => {
        setPost(updatedPostResponse);
        setAuthor(updatedPostResponse.author!);
        setUpdatedContent(updatedPostResponse.content);
        setConfirmationalMessage("Successfully updated Post!");

        toggleEdit(false);

        setTimeout(() => {
          setConfirmationalMessage("");
        }, 3000);
      })
      .catch((error) => setErrorMessage(error.message))
      .finally(() => setIsLoading(false));
  };

  useEffect(() => {
    if (!isEditing) getPost();
  }, [postId, user?.token, lastMessageRegistered, isEditing]);

  return (
    <PageLayout className="details">
      <Container>
        {post && (
          <>
            <div className="details-header mx-auto mb-8 max-w-3xl">
              <h1 className="details-title break-words font-display text-3xl font-medium leading-tight sm:text-4xl">
                {post.title}
              </h1>
              <div className="author-container mt-4 flex flex-wrap gap-x-6 gap-y-2 text-sm text-muted">
                <div className="box flex items-center gap-2">
                  {ICONS.USER_CIRCLE_ICON({})}
                  <p className="author">
                    By {author?.firstName} {author?.lastName}
                  </p>
                </div>
                <div className="box flex items-center gap-2">
                  {ICONS.CLOCK_ICON({})}
                  <p className="date">
                    {DateFunctions.getLocalDateInFormat(post.createdAt!, "DD MMMM YYYY")}
                  </p>
                </div>
              </div>
            </div>

            <div className="details-container mx-auto max-w-3xl">
              {!isEditing && (
                <p className="content min-h-48 rounded-2xl border border-line/70 bg-surface p-6 break-words whitespace-pre-wrap leading-7 sm:p-8">
                  {post.content}
                </p>
              )}
              {isEditing && (
                <TextareaAutosize
                  aria-label="Post content"
                  className="w-full rounded-2xl border border-mint bg-surface p-6 text-ink focus:outline-none focus:ring-2 focus:ring-mint/20 sm:p-8"
                  value={updatedContent}
                  minRows={3}
                  maxRows={20}
                  maxLength={20000}
                  ref={textAreaRef}
                  onChange={(event) => setUpdatedContent(event.currentTarget.value)}
                />
              )}
              <div className="details-update-controls mt-5 flex flex-wrap gap-3">
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
                      variant="secondary"
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
                  variant="secondary"
                  onButtonClick={() => navigate(ROUTES.HOME)}
                />
              </div>
            </div>
          </>
        )}

        {errorMessage && (
          <p role="alert" className="error mx-auto mt-4 max-w-3xl">
            {errorMessage}
          </p>
        )}
        {confirmationalMessage && (
          <p role="status" className="success fade-out mx-auto mt-4 max-w-3xl">
            {confirmationalMessage}
          </p>
        )}
      </Container>
    </PageLayout>
  );
};
