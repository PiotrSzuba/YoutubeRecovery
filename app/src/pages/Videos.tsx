import axios from "axios";
import { FC, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import VideoCard from "../components/videoCard";
import "../index.css"
import { Video } from "../types";

const Videos:FC = () => {

    let navigate = useNavigate();

    const getId = () => {
        return window.location.pathname.replace("/videos/","");
    }

    const [videos,setVideos] = useState<Video[]>([]);
    useEffect(() => {
        window.scroll(0,0);
        axios.get('https://localhost:53523/api/videos/' + getId())
            .then(res => {
                const data = res.data;
                setVideos(data);
            })
            .catch(function (error) {
                if (error.response) {
                    
                }
            });
        }, []);

    return(
        <>
            <div className="btn-black-edgy fixed mt-4 rounded-r-xl px-4" onClick={() => navigate("/playlists/")}>Back to playlists</div>
            <div className="main-container">
                {videos.length != 0 && videos.map((item,index) => 
                    <VideoCard key = {index} 
                            videoId={item.videoId} 
                            playlistId={item.playlistId}
                            ytVideoId={item.ytVideoId}
                            title={item.title}
                            thumbnailUrl={item.thumbnailUrl}
                            postition={item.postition}
                            videoOwnerChannelTitle={item.videoOwnerChannelTitle}
                            videoOwnerChannelId={item.videoOwnerChannelId}
                            privacyStatus={item.privacyStatus}/>)
                }
            </div>        
        </>
    );
}

export default Videos;