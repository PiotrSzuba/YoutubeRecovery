using Newtonsoft.Json;
using YoutubeVideoRecovery.Models;

namespace YoutubeVideoRecovery.Youtube.Playlist;

public class YoutubePlaylist
{
    public Root? root { get; set; }
    public List<Item>? items { get; set; }

    public YoutubePlaylist(Root? root)
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

    public static YoutubePlaylist GetObject(string jsonResponse)
    {
        return new YoutubePlaylist(JsonConvert.DeserializeObject<Root>(jsonResponse));
    }

    public List<Models.Playlist> GetAllPlaylists(User user)
    {
        if (root == null || root.items == null)
        {
            return new List<Models.Playlist>();
        }
        var playlists = new List<Models.Playlist>();

        foreach (var item in root.items)
        {
            if (item.snippet == null ||
                item.snippet.title == null || 
                item.id == null || 
                item.snippet.thumbnails == null || 
                item.snippet.thumbnails.medium == null || 
                item.snippet.thumbnails.medium.url == null)
            {
                continue;
            }
            playlists.Add(new Models.Playlist 
            {
                YtPlaylistId = item.id,
                Title = item.snippet.title,
                ThumbnailUrl = item.snippet.thumbnails.medium.url,
                UserId = user.UserId
            });
        }

        return playlists;
    }

    public List<string> GetPlaylistsIds()
    {
        if (items == null)
        {
            return new();
        }
        var playlistsIds = new List<string>();
        foreach (var item in items)
        {
            playlistsIds.Add(item.id!);
        }

        return playlistsIds;
    }
}

public class Root
{
    public string? kind { get; set; }
    public string? etag { get; set; }
    public string? nextPageToken { get; set; }
    public PageInfo? pageInfo { get; set; }
    public List<Item>? items { get; set; }
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

public class Localized
{
    public string? title { get; set; }
    public string? description { get; set; }
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


public class Snippet
{
    public DateTime publishedAt { get; set; }
    public string? channelId { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public Thumbnails? thumbnails { get; set; }
    public string? channelTitle { get; set; }
    public Localized? localized { get; set; }
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