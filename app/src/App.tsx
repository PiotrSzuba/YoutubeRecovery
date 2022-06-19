import { BrowserRouter, Route, Routes } from "react-router-dom"
import Footer from "./components/footer"
import Navbar from "./components/navbar"
import Home from "./pages/Home"
import { FC } from 'react';
import "./index.css"
import Playlists from "./pages/Playlists";
import Videos from "./pages/Videos";
import { GoogleOAuthProvider } from '@react-oauth/google';

const App:FC = () => {

  return (
    <GoogleOAuthProvider clientId="367716999732-hmmj3sanf3ht2ltcis982uugtbutn2pj.apps.googleusercontent.com">
      <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/playlists" element={<Playlists />} />
        <Route path="/videos/:id" element={<Videos />} />
      </Routes>
      <Footer />
    </BrowserRouter>
  </GoogleOAuthProvider>
  )
}

export default App
