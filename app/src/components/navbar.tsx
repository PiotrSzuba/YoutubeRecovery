import React, { useState,useContext, FC, useEffect } from 'react';
import { NavLink } from "react-router-dom";
import {VscAccount} from 'react-icons/vsc';
import { useNavigate } from "react-router-dom";
import { RiShoppingCartLine} from 'react-icons/ri';
import {IoDocumentTextOutline, IoMenuOutline,IoStatsChart} from 'react-icons/io5';
import {ImWrench} from 'react-icons/im'

const Navbar:FC = () => {
    let navigate = useNavigate(); 

    const [sideMenu, setSideMenu] = useState<boolean>(false);
    const [admin,setAdmin] = useState(true);

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
            <div className='text-4xl cursor-pointer bg-black-900 text-white-100 pl-4 text-center flex flex-col' onClick={handleSideMenu}>
                <div className='my-auto'><IoMenuOutline /></div>
            </div>
            {sideMenu && <>
                <div className='flex flex-col bg-black-900/90 pr-1 fixed top-12 z-50 w-screen items-center'>
                    { admin && <>
                        <div className='navlink-no-center flex mx-4' onClick={() => handleNavigate("/ManageItems")}><ImWrench className='my-auto mx-1'/> Manage items</div>
                        <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/Statistics")}><IoStatsChart className='my-auto mx-1'/>Statistics</div>
                        <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/Statistics")}><IoDocumentTextOutline className='my-auto mx-1'/>Orders</div>
                    </>}
                    <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/Cart")}><RiShoppingCartLine className='my-auto mx-1'/> Cart</div>
                    <div className="navlink-no-center flex mx-4" onClick={() => handleNavigate("/Account")}><VscAccount className='my-auto mx-1'/> Account</div>
                </div>
                <div className='fixed w-screen h-screen bg-black-900/10 z-40 bottom-0' onClick={handleSideMenu}></div>
            </>}
        </div>
    );
}

export default Navbar;