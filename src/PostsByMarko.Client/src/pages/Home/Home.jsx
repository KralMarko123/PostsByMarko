import { React, useState, useEffect, useContext } from "react";
import { useAuth } from "../../custom/useAuth";
import { useSignalR } from "../../custom/useSignalR";
import PostsService from "../../api/PostsService";
import Post from "../../components/Post/Post";
import Nav from "../../components/Layout/Nav/Nav";
import DeletePostForm from "../../components/Forms/DeletePostForm/DeletePostForm";
import UpdatePostForm from "../../components/Forms/UpdatePostForm/UpdatePostForm";
import AppContext from "../../context/AppContext";
import Container from "../../components/Layout/Container/Container";
import Footer from "../../components/Layout/Footer/Footer";
import Logo from "../../components/Layout/Logo/Logo";
import "../Page.css";
import "./Home.css";
import { HelperFunctions } from "../../util/helperFunctions";

const Home = () => {
  const appContext = useContext(AppContext);
  const { user } = useAuth();
  const { lastMessageRegistered } = useSignalR(true);
  const [posts, setPosts] = useState([]);

  const getPosts = async () => {
    await PostsService.getAllPosts(user.token).then((requestResult) => {
      let posts = requestResult.payload;
      HelperFunctions.sortPostsByCreatedDate(posts);

      setPosts(posts);
      appContext.dispatch({ type: "LOAD_POSTS", posts: posts });
    });
  };

  useEffect(() => {
    getPosts();
  }, [lastMessageRegistered, appContext.posts.length]);

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
            <Post
              key={p.id}
              id={p.id}
              authorId={p.authorId}
              title={p.title}
              content={p.content}
              isHidden={p.isHidden}
              allowedUsers={p.allowedUsers}
              createdDate={p.createdDate}
              lastUpdatedDate={p.lastUpdatedDate}
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

export default Home;
