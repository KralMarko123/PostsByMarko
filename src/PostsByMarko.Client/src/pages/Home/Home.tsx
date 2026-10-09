import { PageLayout } from "../../components/Layout/PageLayout/PageLayout";
import { useMemo, useRef, useEffect, useContext, useState } from "react";
import { useAuth } from "../../custom/useAuth";
import { PostService } from "../../api/PostService";
import { DateFunctions } from "../../util/dateFunctions";
import { PostCard } from "../../components/Post/PostCard";
import { DeletePostForm } from "../../components/Forms/DeletePostForm/DeletePostForm";
import { UpdatePostForm } from "../../components/Forms/UpdatePostForm/UpdatePostForm";
import { AppContext } from "../../context/AppContext";
import { Container } from "../../components/Layout/Container/Container";

export const Home = () => {
  const appContext = useContext(AppContext);
  const { user, checkToken } = useAuth();
  const requestVersion = useRef(0);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const posts = useMemo(() => {
    const sorted = [...appContext.posts];
    DateFunctions.sortItemsByDateTimeAttribute(sorted, "createdAt");
    return sorted;
  }, [appContext.posts]);

  const getPosts = async () => {
    const version = ++requestVersion.current;
    setIsLoading(true);
    await PostService.getPosts(user!.token!)
      .then((posts) => {
        if (version !== requestVersion.current) return;
        setErrorMessage("");
        appContext.dispatch({ type: "LOAD_POSTS", posts: posts });
      })
      .catch(async (error) => {
        if (version !== requestVersion.current) return;
        setErrorMessage(error.message);
        await checkToken();
      })
      .finally(() => {
        if (version === requestVersion.current) setIsLoading(false);
      });
  };

  useEffect(() => {
    getPosts();
    return () => {
      requestVersion.current++;
    };
  }, [appContext.lastMessageRegistered, user?.token]);

  return (
    <PageLayout className="home">
      <Container
        title="Today's Posts"
        desc="Check out what's going on with the world. Create, edit & inspire"
      >
        <ul className="posts-list grid grid-cols-1 gap-5 md:grid-cols-2 xl:grid-cols-3">
          {posts?.map((p, i) => (
            <PostCard
              key={p.id}
              id={p.id}
              authorId={p.authorId}
              author={p.author}
              title={p.title}
              content={p.content}
              hidden={p.hidden}
              createdAt={p.createdAt}
              lastUpdatedAt={p.lastUpdatedAt}
              index={i}
            />
          ))}
        </ul>
        {errorMessage && (
          <p className="error mt-5" role="alert">
            {errorMessage}
          </p>
        )}
        {isLoading && posts.length === 0 && (
          <p className="text-muted" role="status">
            Loading posts…
          </p>
        )}
        {!isLoading && !errorMessage && posts.length === 0 && (
          <p className="rounded-2xl border border-dashed border-line p-10 text-center text-muted">
            No posts yet. Open the menu to share the first one.
          </p>
        )}

        <UpdatePostForm />
        <DeletePostForm />
      </Container>
    </PageLayout>
  );
};
