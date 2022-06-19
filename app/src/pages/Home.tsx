import { FC, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { googleLogout, CodeResponse,useGoogleLogin, TokenResponse } from '@react-oauth/google';
import "../index.css"
import axios from "axios";

const Home:FC = () => {
    let navigate = useNavigate(); 

    const sendDataToBackend = (codeResponse: TokenResponse) => {
        console.log(JSON.stringify(codeResponse));
        axios.post('https://localhost:53523/api/login/' , codeResponse)
            .then(res => {
                const data = res.data;
                console.log(data);
            })
            .catch((error) => {
                if (error.response) {
                    console.log(error.response);
                }
            });
    }

    const login = useGoogleLogin({
        onSuccess: (codeResponse) => sendDataToBackend(codeResponse),
        //scope: "https://www.googleapis.com/auth/youtube"
      });

    return(
        <div className="main-container">
            <div className = "flex flex-col">
                <div className="btn-black w-1/2 mx-auto my-8" onClick={() => navigate("/playlists")}>
                    My Playlists
                </div>
                <div className="btn-black w-1/2 mx-auto my-8" onClick={() => login()}>
                    Sign in to youtube
                </div>
                <div className="btn-black w-1/2 mx-auto my-8" onClick={() => googleLogout()}>
                    Logout
                </div>
            </div>
        </div>
    );
}

export default Home;