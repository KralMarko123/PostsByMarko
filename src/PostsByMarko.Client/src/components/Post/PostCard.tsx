import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { ICONS, EYE_ICON } from "../../constants/icons";
import { DateFunctions } from "../../util/dateFunctions";
import { PostService } from "../../api/PostService";
import { ROUTES } from "../../constants/routes";
import { AppContext } from "../../context/AppContext";
import { Card } from "../Helper/Card/Card";
import { Post, PostProps } from "@typeConfigs/post";
import "./PostCard.css";

export const PostCard = ({
  id,
  authorId,
  author,
  title,
  content,
  hidden,
  createdAt,
  lastUpdatedAt,
  index,
}: PostProps) => {
  let navigate = useNavigate();
  const { user, isAdmin } = useAuth();
  const appContext = useContext(AppContext);
  const [post, setPost] = useState<Post>({
    id: id,
    authorId: authorId,
    author: author,
    title: title,
    content: content,
    hidden: hidden,
    createdAt: createdAt,
    lastUpdatedAt: lastUpdatedAt,
  });
  const isAuthor: boolean = post.authorId === user?.id;
  const readableCreatedDate: string = DateFunctions.getReadableDateTime(post.createdAt!);

  const handlePostClick = () => {
    navigate(`.${ROUTES.DETAILS}/${id}`);
  };

  const handleModalToggle = (
    e: React.MouseEvent<HTMLSpanElement>,
    modalToToggle: string
  ) => {
    e.stopPropagation();

    appContext.dispatch({
      type: "MODIFYING_POST",
      post: post,
    });

    appContext.dispatch({ type: "SHOW_MODAL", modal: modalToToggle });
  };

  const handleHiddenToggle = async (e: React.MouseEvent<HTMLSpanElement>) => {
    e.stopPropagation();

    appContext.dispatch({
      type: "MODIFYING_POST",
      post: post,
    });

    let updateRequest = { title, content, hidden: !hidden };

    await PostService.updatePost(id!, updateRequest, user!.token!)
      .then((updatedPost) => {
        setPost(updatedPost);

        appContext.dispatch({
          type: "UPDATED_POST",
          post: updatedPost,
        });
      })
      .catch((error) => {
        // TODO: Create modal notification that something went wrong
        console.log(error);
      });
  };

  useEffect(() => {
    setPost({
      id: id,
      authorId: authorId,
      author: author,
      title: title,
      content: content,
      hidden: hidden,
      createdAt: createdAt,
      lastUpdatedAt: lastUpdatedAt,
    });
  }, [title, content, hidden, lastUpdatedAt]);

  return (
    <Card>
      <div
        id={`post-${id}`}
        className={`post ${post.hidden ? "hidden" : ""}`}
        onClick={() => handlePostClick()}
        style={{ animationDelay: `${index * 0.15}s` }}
      >
        {(isAuthor || isAdmin) && (
          <span className="post-icon hide" onClick={(e) => handleHiddenToggle(e)}>
            {EYE_ICON(post.hidden)}
          </span>
        )}
        {(isAuthor || isAdmin) && (
          <span
            className="post-icon update"
            onClick={(e) => handleModalToggle(e, "updatePost")}
          >
            {ICONS.PENCIL_ICON!({})}
          </span>
        )}
        {(isAuthor || isAdmin) && (
          <span
            className="post-icon delete"
            onClick={(e) => handleModalToggle(e, "deletePost")}
          >
            {ICONS.DELETE_ICON!({})}
          </span>
        )}

        <span className="post-date">{readableCreatedDate}</span>
        <h1 className="post-title">{title}</h1>
        <p className="post-content">{content}</p>
      </div>
    </Card>
  );
};
