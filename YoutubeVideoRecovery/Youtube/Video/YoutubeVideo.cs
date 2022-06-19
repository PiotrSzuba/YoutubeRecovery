using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeVideoRecovery.Youtube.VideoStatus;

namespace YoutubeVideoRecovery.Youtube.Video;

public class YoutubeVideo
{
    public Root? root { get; set; }
    public List<Item>? items { get; set; }
    public YoutubeVideo(Root? root)
    {
        this.root = root;
        if (root == null || root.items == null)
        {
            return;
        }
        var items = new List<Item>();
        foreach (var item in root.items)
        {
            if (item == null)
            {
                continue;
            }
            items.Add(item);
        }
        this.items = items ?? (this.items = new List<Item>());
    }
    public static YoutubeVideo GetObject(string jsonResponse)
    {
        return new YoutubeVideo(JsonConvert.DeserializeObject<Root>(jsonResponse));
    }

    public List<Models.Video> GetAllVideos(int playListId, YoutubeVideoStatus statuses)
    {
        if (root == null || root.items == null)
        {
            return new List<Models.Video>();
        }
        var playlists = new List<Models.Video>();

        foreach (var item in root.items)
        {
            if (item.snippet == null ||
                item.snippet.title == null ||
                item.id == null ||
                item.snippet.thumbnails == null ||
                item.snippet.thumbnails.medium == null ||
                item.snippet.thumbnails.medium.url == null ||
                item.snippet.resourceId == null || 
                item.snippet.resourceId.videoId == null)
            {
                continue;
            }
            playlists.Add(new Models.Video
            {
                YtVideoId = item.snippet.resourceId.videoId,
                Title = item.snippet.title,
                ThumbnailUrl = item.snippet.thumbnails.medium.url,
                Postition = item.snippet.position,
                VideoOwnerChannelTitle = item.snippet.videoOwnerChannelTitle,
                VideoOwnerChannelId = item.snippet.videoOwnerChannelId,
                PlaylistId = playListId,
                PrivacyStatus = statuses.GetVideoPrivacyStatus(item.snippet.resourceId.videoId!)
            });
        }

        return playlists;
    }

    public List<string> GetVideosIds()
    {
        if (root == null || root.items == null)
        {
            return new List<string>();
        }
        var ids = new List<string>();

        foreach (var item in root.items)
        {
            if (item.snippet == null || item.snippet.resourceId == null || item.snippet.resourceId.videoId == null)
            {
                continue;
            }
            ids.Add(item.snippet.resourceId.videoId);
        }

        return ids;
    }
}

public class Default
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class High
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Item
{
    public string? kind { get; set; }
    public string? etag { get; set; }
    public string? id { get; set; }
    public Snippet? snippet { get; set; }
}

public class Maxres
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Medium
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class PageInfo
{
    public int totalResults { get; set; }
    public int resultsPerPage { get; set; }
}

public class ResourceId
{
    public string? kind { get; set; }
    public string? videoId { get; set; }
}

public class Root
{
    public string? kind { get; set; }
    public string? etag { get; set; }
    public string? nextPageToken { get; set; }
    public string? prevPageToken { get; set; }
    public List<Item>? items { get; set; }
    public PageInfo? pageInfo { get; set; }
}

public class Snippet
{
    public DateTime publishedAt { get; set; }
    public string? channelId { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public Thumbnails? thumbnails { get; set; }
    public string? channelTitle { get; set; }
    public string? playlistId { get; set; }
    public int position { get; set; }
    public ResourceId? resourceId { get; set; }
    public string? videoOwnerChannelTitle { get; set; }
    public string? videoOwnerChannelId { get; set; }
}

public class Standard
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Thumbnails
{
    public Default? @default { get; set; }
    public Medium? medium { get; set; }
    public High? high { get; set; }
    public Standard? standard { get; set; }
    public Maxres? maxres { get; set; }
}