import React, {FC,useState, useEffect} from "react";
import {SiCsharp, SiDotnet, SiMicrosoftsqlserver, SiReact, SiTailwindcss, SiTypescript, SiVite} from 'react-icons/si'
import { VscGithub } from "react-icons/vsc";


const Footer:FC = () => {

    const externalPage = (link:string):void => {
        window.location.assign(link);
    }

    return (
        <div className="bg-black-900 py-8">
            <div className = "m-auto mb-12 w-2/3 grid grid-flow-col gap-2 text-white-100 text-center">
                <div className="flex flex-col">
                    <div className="mb-4 cursor-default">Informations</div>
                    <div className="text-sm mb-2 text-white-600 cursor-pointer">Privacy policy</div>
                    <div className="text-sm mb-2 text-white-600 cursor-pointer">Contact form</div>
                </div>
                <div className="flex flex-col">
                    <div className="mb-4 cursor-default">About me</div>
                </div>
                <div className="flex flex-col">
                    <div className="mb-4 cursor-default">Social network</div>
                    <div className="flex flex-row mx-auto cursor-pointer text-white-600" onClick={() => externalPage("https://github.com/PiotrSzuba")} >
                        <VscGithub className="my-auto mr-1" />
                        <div className=" text-sm">P.Sz</div>
                    </div>
                </div>
            </div>
            <div className = "w-2/3 bg-black-300 h-1px m-auto"></div>
            <div className="flex flex-row w-2/3 mx-auto mt-4 text-white-600">
                <div className=" font-comforter text-white-100 text-2xl font-medium cursor-pointer">Merch shop</div>
                <div className=" ml-auto flex flex-row text-md">
                    <div className="my-auto">Techstack</div>
                    <SiCsharp className="mx-1 my-auto"/>
                    <SiDotnet className="mx-1 my-auto"/>
                    <SiMicrosoftsqlserver className="mx-1 my-auto"/>
                    <SiVite className="mx-1 my-auto"/>
                    <SiReact className="mx-1 my-auto"/>
                    <SiTypescript className="mx-1 my-auto"/>
                    <SiTailwindcss className=" text-white-100 mx-1 my-auto"/>
                </div>         
            </div>
        </div>
    );

}

export default Footer;