import { FC } from "react";
import { NavLink } from "react-router-dom";
import { Playlist } from "../types";


const PlaylistCard:FC<Playlist> = ({playlistId,ytPlaylistId,title,thumbnailUrl}: Playlist) => {
    return (
        <div className='m-auto'>
            <div className='card-container hover:ring-1 ring-black-500 rounded-b-xl'>
                <NavLink className = "navlink-stripper" to = {"/videos/" + playlistId}>
                        <img className= "card-photo" alt = "No content" src={thumbnailUrl}/>
                        <div className="card-description font-medium"> 
                            <div className='text-center'>
                                {title}
                            </div>
                        </div>
                    </NavLink>
            </div>
        </div>
    );
}

export default PlaylistCard;