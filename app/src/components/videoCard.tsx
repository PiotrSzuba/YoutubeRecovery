import { FC } from "react";
import { Video } from "../types";

const VideoCard:FC<Video> = ({videoId,playlistId,ytVideoId,title,thumbnailUrl,postition,videoOwnerChannelTitle,videoOwnerChannelId,privacyStatus}:Video) => {

    const privacyClassName:string =  privacyStatus === "public" ? "text-green-500" : privacyStatus.toLocaleLowerCase().includes("private") ? "text-red-500" : "text-green-200";

    return(
        <div className="flex flex-row my-4 hover:ring-1 hover:ring-black-300 hover:rounded-r-xl xs:flex-col sm:flex-row">
            <img className= "xs:w-96 xs:mx-auto sm:mx-0 sm:w-auto" alt = "No content" src={thumbnailUrl}/>
            <div className="ml-4 flex flex-col my-auto">
                <div className=" font-semibold text-xl">{title}</div>
                <div className=" text-lg">{videoOwnerChannelTitle}</div>
                <div className={privacyClassName}>{privacyStatus}</div>
            </div>
        </div>
    );
}

export default VideoCard;