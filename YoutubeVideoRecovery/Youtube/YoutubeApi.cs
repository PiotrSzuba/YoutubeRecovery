using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.Youtube.Video;
using YoutubeVideoRecovery.Youtube.VideoStatus;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Youtube.Playlist;

namespace YoutubeVideoRecovery.Youtube;

public class YoutubeApi
{
    private static readonly HttpClient client = new();
    private static readonly string apiKey = "AIzaSyBNERAvX6eIlF - 9CvsNsuPIVUEWvq8zk5U";
    public async static Task<List<Models.Playlist>> GetPlaylists(User user)
    {
        if(user.ChannelId == null || user.ChannelId.Length != 24)
        {
            return new();
        }
        List<Models.Playlist> playlists = new();
        var channelId = user.ChannelId;
        var url = $"https://www.googleapis.com/youtube/v3/playlists?part=snippet&maxResults=50&channelId={channelId}&key={apiKey}";
        var stringTask = await client.GetStringAsync(url);
        var playlistsObject = YoutubePlaylist.GetObject(stringTask);
        playlists.AddRange(playlistsObject.GetAllPlaylists(user));

        if (playlistsObject.root != null && playlistsObject.root.nextPageToken != null)
        {
            var iterations = (int)Math.Floor((double)playlistsObject.root.pageInfo!.totalResults / 50);
            for (int i = 0; i < iterations; i++)
            {
                url = $"https://www.googleapis.com/youtube/v3/playlists?part=snippet&maxResults=50&pageToken={playlistsObject.root!.nextPageToken}&channelId={channelId}&key={apiKey}";
                stringTask = await client.GetStringAsync(url);
                playlistsObject = YoutubePlaylist.GetObject(stringTask);
                playlists.AddRange(playlistsObject.GetAllPlaylists(user));
            }
        }

        return playlists;
    }

    public async static Task<List<Models.Video>> GetVideosInPlaylist(string YtPlaylistId, int playlistId)
    {
        List<Models.Video> videos = new(); 
        var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId={YtPlaylistId}&key={apiKey}";
        var stringTask = await client.GetStringAsync(url);
        var videosObject = YoutubeVideo.GetObject(stringTask);
        var statusesJson = await GetVideoPrivacyStatus(string.Join(",", videosObject.GetVideosIds()));
        var statusesObject = YoutubeVideoStatus.GetObject(statusesJson);
        videos.AddRange(videosObject.GetAllVideos(playlistId, statusesObject));

        if (videosObject.root != null && videosObject.root.nextPageToken != null)
        {
            var iterations = (int)Math.Floor((double)videosObject.root.pageInfo!.totalResults / 50);
            for (int i = 0; i < iterations; i++)
            {
                url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&pageToken={videosObject.root!.nextPageToken}&playlistId={YtPlaylistId}&key={apiKey}";
                stringTask = await client.GetStringAsync(url);
                videosObject = YoutubeVideo.GetObject(stringTask);
                statusesJson = await GetVideoPrivacyStatus(string.Join(",", videosObject.GetVideosIds()));
                statusesObject = YoutubeVideoStatus.GetObject(statusesJson);
                videos.AddRange(videosObject.GetAllVideos(playlistId, statusesObject));
            }
        }

        return videos;
    }

    

    public async static Task<string> GetVideoPrivacyStatus(string videoId)
{
        var url = $"https://www.googleapis.com/youtube/v3/videos?part=status&maxResults=50&id={videoId}&key={apiKey}";
        var stringTask = await client.GetStringAsync(url);

        return stringTask;
    }
}
