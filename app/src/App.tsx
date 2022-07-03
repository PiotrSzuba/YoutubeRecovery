import { BrowserRouter, Route, Routes } from "react-router-dom"
import Footer from "./components/footer"
import Navbar from "./components/navbar"
import Home from "./pages/Home"
import { FC, useState, useEffect } from 'react';
import "./index.css"
import Playlists from "./pages/Playlists";
import Videos from "./pages/Videos";
import { GoogleOAuthProvider } from '@react-oauth/google';
import { userContext } from './contexts/userContext';
import { User } from "./types";
import axios from "axios";
import baseUrl from "./config";

const App:FC = () => {
  const [user,setUser] = useState<User>();

  const handleUserChange = (user?:User) => {
    if(user){
      localStorage.setItem("user",JSON.stringify(user));
      axios.defaults.headers.common["user"] = user.userId;
    }
    setUser(user);
  }

  useEffect(() => {
    const localUser = localStorage.getItem("user");
    if(!localUser){
      return;
    }
    const parsedUser:User = JSON.parse(localUser);
    if(!parsedUser){
      return;
    }
    axios.get(baseUrl + '/account/' + parsedUser.userId)
        .then(res => {
          const User:User = res.data;
          handleUserChange(User);
        })
        .catch((error) =>  {
            if (error.response) {
                console.log(error.response);
            }
        });
    }, []);

  return (
    <GoogleOAuthProvider clientId="367716999732-hmmj3sanf3ht2ltcis982uugtbutn2pj.apps.googleusercontent.com">
      <userContext.Provider value = {{user,handleUserChange}} >
      <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/playlists" element={<Playlists />} />
        <Route path="/videos/:id" element={<Videos />} />
      </Routes>
      <Footer />
    </BrowserRouter>
    </userContext.Provider>
  </GoogleOAuthProvider>
  )
}

export default App
