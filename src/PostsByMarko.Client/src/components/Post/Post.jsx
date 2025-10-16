import { React, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/icons";
import { useSignalR } from "../../custom/useSignalR";
import * as ROUTES from "../../constants/routes";
import AppContext from "../../context/AppContext";
import PostsService from "../../api/PostsService";
import Card from "../Helper/Card/Card";
import { DateFunctions } from "../../util/dateFunctions";
import "./Post.css";

const Post = ({
  id,
  authorId,
  title,
  content,
  isHidden,
  createdDate,
  index,
}) => {
  let navigate = useNavigate();
  const appContext = useContext(AppContext);
  const { user, isAdmin } = useAuth();
  const isAuthor = authorId === user.id;
  const readableCreatedDate = DateFunctions.getReadableDateTime(createdDate);
  const { sendMessage } = useSignalR(true);

  const handlePostClick = () => {
    navigate(`.${ROUTES.DETAILS_PREFIX}/${id}`);
  };

  const handleModalToggle = (e, modalToToggle) => {
    e.stopPropagation();
    appContext.dispatch({
      type: "MODIFYING_POST",
      post: { id: id, title: title, content: content },
    });
    appContext.dispatch({ type: "SHOW_MODAL", modal: modalToToggle });
  };

  const handleHiddenToggle = async (e) => {
    e.stopPropagation();

    await PostsService.togglePostVisibility(id, user.token).then(
      (requestResult) => {
        if (requestResult.statusCode === 200) {
          appContext.dispatch({ type: "TOGGLE_POST_HIDDEN", id: id });
          sendMessage({
            message: `Modified visibility for post with Id: ${id}`,
            toAll: true,
          });
        }
      }
    );
  };

  return (
    <Card>
      <div
        id={`post-${id}`}
        className={`post ${isHidden ? "hidden" : ""}`}
        onClick={() => handlePostClick()}
        style={{ animationDelay: `${index * 0.15}s` }}
      >
        {(isAuthor || isAdmin) && (
          <span
            className="post-icon hide"
            onClick={(e) => handleHiddenToggle(e)}
          >
            {ICONS.EYE_ICON(isHidden)}
          </span>
        )}
        {(isAuthor || isAdmin) && (
          <span
            className="post-icon update"
            onClick={(e) => handleModalToggle(e, "updatePost")}
          >
            {ICONS.PENCIL_ICON()}
          </span>
        )}
        {(isAuthor || isAdmin) && (
          <span
            className="post-icon delete"
            onClick={(e) => handleModalToggle(e, "deletePost")}
          >
            {ICONS.DELETE_ICON()}
          </span>
        )}

        <span className="post-date">{readableCreatedDate}</span>
        <h1 className="post-title">{title}</h1>
        <p className="post-content">{content}</p>
      </div>
    </Card>
  );
};

export default Post;
