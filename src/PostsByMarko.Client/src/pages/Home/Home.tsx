import { useState, useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import { PostService } from "../../api/PostService";
import { DateFunctions } from "../../util/dateFunctions";
import { PostCard } from "../../components/Post/PostCard";
import { Nav } from "../../components/Layout/Nav/Nav";
import { DeletePostForm } from "../../components/Forms/DeletePostForm/DeletePostForm";
import { UpdatePostForm } from "../../components/Forms/UpdatePostForm/UpdatePostForm";
import { AppContext } from "../../context/AppContext";
import { Container } from "../../components/Layout/Container/Container";
import { Footer } from "../../components/Layout/Footer/Footer";
import { Logo } from "../../components/Layout/Logo/Logo";
import { Post } from "@typeConfigs/post";
import "../Page.css";
import "./Home.css";

export const Home = () => {
  const appContext = useContext(AppContext);
  const { user, checkToken } = useAuth();
  const [posts, setPosts] = useState<Post[]>([]);

  const getPosts = async () => {
    await PostService.getPosts(user!.token!)
      .then((posts) => {
        DateFunctions.sortItemsByDateTimeAttribute(posts, "createdAt");
        setPosts(posts);
        appContext.dispatch({ type: "LOAD_POSTS", posts: posts });
      })
      .catch(async (error) => {
        await checkToken();

        // TODO: Add some kind of global notification
        console.log(error);
      });
  };

  useEffect(() => {
    getPosts();
  }, [appContext.lastMessageRegistered, appContext.posts.length]);

  return (
    <div className="home page">
      <Logo />
      <Nav />

      <Container
        title="Today's Posts"
        desc="Check out what's going on with the world. Create, edit & inspire"
      >
        <ul className="posts-list">
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

        <UpdatePostForm />
        <DeletePostForm />
      </Container>

      <Footer />
    </div>
  );
};
