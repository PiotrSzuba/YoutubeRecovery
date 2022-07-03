import { FC, useEffect, useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { googleLogout, CodeResponse,useGoogleLogin, TokenResponse } from '@react-oauth/google';
import "../index.css"
import axios from "axios";
import { User } from "../types";
import { userContext } from "../contexts/userContext";
import baseUrl from "../config";


const Home:FC = () => {
    let navigate = useNavigate(); 

    const {user,handleUserChange} = useContext(userContext);

    const [channelId, setChannelId] = useState<string>();
    const [errorVisible, setErrorVisible] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string>("");

    const sendDataToBackend = (tokenResponse: TokenResponse) => {
        axios.post(baseUrl + "/account/login/" , tokenResponse)
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

    const handleChannelIdChange = (value:string) => {
        setChannelId(value);
    }

    const login = useGoogleLogin({
        onSuccess: (tokenResponse) => sendDataToBackend(tokenResponse),
        //scope: "https://www.googleapis.com/auth/youtube" //google app verification needed
      });

    const confirmChannelId = () => {
        if(!user || !channelId || !handleUserChange){
            return;
        }
        if(channelId.length != 24){
            showError("Channel id should have 24 characters !");
            return;
        }
        let updatedUser = user;
        updatedUser.channelId = channelId;
        axios.post(baseUrl + '/account/addchannel/' , updatedUser)
        .then(res => {
            const data = res.data;
            if(!data || !handleUserChange){
                return;
            }
            handleUserChange(data);
        })
        .catch((error) => {
            if (error.response) {
                console.log(error.response);
            }
        });
    }

    const logout = () => {
        if(handleUserChange){
            handleUserChange(undefined);
            localStorage.clear();
        }
    }

    const showError = (message:string) => {
        setErrorMessage(message);
        setErrorVisible(true);
        setTimeout(() => {
            setErrorVisible(false);
            setErrorMessage("");
          }, 2000);
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
                    <div>Channel ID: {!user.channelId ? "You need to spicify your channel id" : user.channelId}</div>
                </div>
            </div>
            { user.channelId && 
                <div className="btn-black w-1/2 mx-auto my-4" onClick={() => navigate("/playlists")}>
                    My Playlists
                </div>
            }
            { !user.channelId && <>
                <div className="w-1/2 mx-auto my-4">
                    <input className="input" value={channelId} type="text" placeholder="Type your channel id" onChange={(e) => handleChannelIdChange(e.target.value)}/>
                </div>
                <div className="btn-black w-1/2 mx-auto my-4" onClick={() => confirmChannelId()}>
                    Confirm
                </div>
            </>}
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
            { errorVisible &&
                <div className=" text-center text-red-500">
                    {errorMessage}
                </div>
            }
        </div>
    );
}

export default Home;