import { FC, useEffect, useState } from "react";
import axios from 'axios';
import "../index.css"
import { Playlist } from "../types";
import PlaylistCard from "../components/playlistCard";

const Playlists:FC = () => {
    const [playlists,setPlaylists] = useState<Playlist[]>([]);
    useEffect(() => {
        window.scroll(0,0);
        axios.get('https://localhost:53523/api/playlist')
            .then(res => {
                const data = res.data;
                setPlaylists(data);
            })
            .catch(function (error) {
                if (error.response) {
                    
                }
            });
        }, []);


    return(
        <div className="main-container">
            <div className = "main-grid gap-4">
                {playlists.length != 0 && playlists.map((item,index) => 
                    <PlaylistCard key = {index} playlistId={item.playlistId} ytPlaylistId={item.ytPlaylistId} title={item.title} thumbnailUrl={item.thumbnailUrl}/>)
                }
            </div>
        </div>
    );
}

export default Playlists;