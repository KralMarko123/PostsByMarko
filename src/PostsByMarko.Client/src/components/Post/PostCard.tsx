import { useContext, useEffect, useState, useRef } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../custom/useAuth";
import { ICONS } from "../../constants/icons";
import { DateFunctions } from "../../util/dateFunctions";
import { PostService } from "../../api/PostService";
import { POST_DETAILS_PREFIX } from "../../constants/routes";
import { AppContext } from "../../context/AppContext";
import { Card } from "../Helper/Card/Card";
import { Post, PostProps } from "@typeConfigs/post";

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
  const updatingVisibility = useRef(false);
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

  const handleModalToggle = (e: React.MouseEvent<HTMLButtonElement>, modalToToggle: string) => {
    e.stopPropagation();

    appContext.dispatch({
      type: "MODIFYING_POST",
      post: post,
    });

    appContext.dispatch({ type: "SHOW_MODAL", modal: modalToToggle });
  };

  const handleHiddenToggle = async (e: React.MouseEvent<HTMLButtonElement>) => {
    e.stopPropagation();

    appContext.dispatch({
      type: "MODIFYING_POST",
      post: post,
    });

    if (updatingVisibility.current) return;
    updatingVisibility.current = true;
    const updateRequest = { title: post.title, content: post.content, hidden: !post.hidden };

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
      })
      .finally(() => {
        updatingVisibility.current = false;
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
    <li
      className="h-full motion-safe:animate-appear"
      style={{ animationDelay: `${Math.min(index, 6) * 0.04}s` }}
    >
      <Card className="h-full transition-colors hover:border-mint/50">
        <article
          id={`post-${id}`}
          data-hidden={post.hidden}
          className="post relative flex h-full min-h-64 flex-col gap-4 p-6 data-[hidden=true]:opacity-60"
        >
          <div className="flex flex-wrap items-center gap-2 text-xs text-muted">
            <span className="post-date">{readableCreatedDate}</span>
            {post.hidden && (
              <span className="rounded-full border border-line px-2 py-0.5">Hidden</span>
            )}
          </div>
          <h2 className="post-title line-clamp-2 font-display text-2xl font-medium leading-snug">
            <Link
              to={`${POST_DETAILS_PREFIX}/${id}`}
              className="after:absolute after:inset-0 after:rounded-2xl"
            >
              {post.title}
            </Link>
          </h2>
          <p className="post-content line-clamp-3 break-words text-muted">{post.content}</p>
          <div className="mt-auto flex flex-wrap items-center justify-between gap-2 border-t border-line/50 pt-3">
            <span className="text-xs text-muted">
              {post.author?.firstName} {post.author?.lastName}
            </span>
            {(isAuthor || isAdmin) && (
              <div className="relative z-10 flex items-center gap-1">
                <button
                  type="button"
                  className="post-icon hide grid size-11 place-items-center rounded-lg text-muted hover:bg-elevated hover:text-mint"
                  aria-label={`${post.hidden ? "Show" : "Hide"} post: ${post.title}`}
                  onClick={handleHiddenToggle}
                >
                  {post.hidden ? <ICONS.EYE_ICON_INVISIBLE /> : <ICONS.EYE_ICON />}
                </button>
                <button
                  type="button"
                  className="post-icon update grid size-11 place-items-center rounded-lg text-muted hover:bg-elevated hover:text-mint"
                  aria-label={`Edit post: ${post.title}`}
                  onClick={(event) => handleModalToggle(event, "updatePost")}
                >
                  {ICONS.PENCIL_ICON!({})}
                </button>
                <button
                  type="button"
                  className="post-icon delete grid size-11 place-items-center rounded-lg text-muted hover:bg-elevated hover:text-danger"
                  aria-label={`Delete post: ${post.title}`}
                  onClick={(event) => handleModalToggle(event, "deletePost")}
                >
                  {ICONS.DELETE_ICON!({})}
                </button>
              </div>
            )}
          </div>
        </article>
      </Card>
    </li>
  );
};
