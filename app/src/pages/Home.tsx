import { FC, useEffect, useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { googleLogout, CodeResponse,useGoogleLogin, TokenResponse } from '@react-oauth/google';
import "../index.css"
import axios from "axios";
import { User } from "../types";
import { userContext } from "../contexts/userContext";

const Home:FC = () => {
    let navigate = useNavigate(); 

    const {user,handleUserChange} = useContext(userContext);

    const sendDataToBackend = (tokenResponse: TokenResponse) => {
        axios.post('https://localhost:53523/api/login/' , tokenResponse)
            .then(res => {
                const data = res.data;
                if(data && handleUserChange){
                    handleUserChange(data);
                }
            })
            .catch((error) => {
                if (error.response) {
                    console.log(error.response);
                }
            });
    }

    const login = useGoogleLogin({
        onSuccess: (tokenResponse) => sendDataToBackend(tokenResponse),
        //scope: "https://www.googleapis.com/auth/youtube"
      });

    const logout = () => {
        if(handleUserChange){
            handleUserChange(undefined);
        }
    }


    return(
        <div className="main-container">
            {user &&
            <>
            <div className='w-full flex flex-row justify-center'>
                <img className = "" alt = "Loading" src={user.pictureUrl}/>
                <div className='flex flex-col mx-4 my-auto'>
                    <div className='text-2xl'>{user.username}</div>
                    <div className='text-xl'>{user.email}</div>
                    <div className='text-xl'>Videos recovered: {user.recoveredVideos}</div>
                </div>
            </div>
            <div className="btn-black w-1/2 mx-auto my-4" onClick={() => navigate("/playlists")}>
                My Playlists
            </div>
            <div className="btn-black w-1/2 mx-auto my-4" onClick={() => logout()}>
                Logout
            </div>
            </>
            }
            {!user && 
                <div className="btn-black w-1/2 mx-auto my-8" onClick={() => login()}>
                    Sign in to youtube
                </div>
            }
        </div>
    );
}

export default Home;