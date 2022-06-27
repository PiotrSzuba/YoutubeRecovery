export type Playlist = {
    playlistId: number;
    ytPlaylistId: string;
    title: string;
    thumbnailUrl: string;
}

export type Video = {
    videoId: number;
    playlistId: number;
    ytVideoId: string;
    title: string;
    thumbnailUrl: string;
    postition: number;
    videoOwnerChannelTitle: string;
    videoOwnerChannelId: string;
    privacyStatus: string;
}

export type User = {
    userId: number;
    username: string;
    email: string;
    pictureUrl: string;
    locale: string;
    recoveredVideos: number;
    channelId: string;
}