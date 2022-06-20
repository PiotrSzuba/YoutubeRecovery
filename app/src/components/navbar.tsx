import React, { useState,useContext, FC, useEffect } from 'react';
import { NavLink } from "react-router-dom";
import {VscAccount} from 'react-icons/vsc';
import { useNavigate } from "react-router-dom";
import {IoDocumentTextOutline, IoMenuOutline} from 'react-icons/io5';
import {ImWrench} from 'react-icons/im'
import { userContext } from "../contexts/userContext";

const Navbar:FC = () => {
    let navigate = useNavigate(); 

    const {user,handleUserChange} = useContext(userContext);

    const [sideMenu, setSideMenu] = useState<boolean>(false);

    const handleSideMenu= ():void  => {
        if(sideMenu){
            setSideMenu(false);
            return;
        }
        setSideMenu(true);
    }

    const handleNavigate = (path:string):void => {
        navigate(path);
        setSideMenu(false);
    }

    return (
        <div className='navbar-container flex flex-row'>
            <div className="navbar-content">
                <div className="navLink font-comforter text-2xl" onClick={() => handleNavigate("/")}>Youtube recovery</div>
            </div>
            {user && 
            <div className='text-4xl cursor-pointer bg-black-900 text-white-100 pl-4 text-center flex flex-col' onClick={handleSideMenu}>
                <div className='my-auto'><IoMenuOutline /></div>
            </div>
            }
            {sideMenu && <>
                <div className='flex flex-col bg-black-900/90 pr-1 fixed top-12 z-50 w-screen items-center'>
                    { user && <>
                        <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/playlists")}><IoDocumentTextOutline className='my-auto mx-1'/> Playlists</div>
                        <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/")}><VscAccount className='my-auto mx-1'/> Account</div>
                    </>}

                </div>
                {user && 
                    <div className='fixed w-screen h-screen bg-black-900/10 z-40 bottom-0' onClick={handleSideMenu}></div>
                }
            </>}
        </div>
    );
}

export default Navbar;