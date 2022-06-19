using Newtonsoft.Json;
using YoutubeVideoRecovery.Youtube.Video;

namespace YoutubeVideoRecovery.Youtube.VideoStatus;

public class YoutubeVideoStatus
{
    public Root? root { get; set; }
    public List<Item>? items { get; set; }
    public YoutubeVideoStatus(Root? root)
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
    public static YoutubeVideoStatus GetObject(string jsonResponse)
    {
        return new YoutubeVideoStatus(JsonConvert.DeserializeObject<Root>(jsonResponse));
    }

    public string GetVideoPrivacyStatus(string VideoId)
    {
        if (root == null || root.items == null)
        {
            return "deleted";
        }
        foreach (var item in root.items)
        {
            if (item.id == VideoId)
            {
                return item.status!.privacyStatus!;
            }
        }

        return "deleted";

    }
}

public class Item
{
    public string? kind { get; set; }
    public string? etag { get; set; }
    public string? id { get; set; }
    public Status? status { get; set; }
}

public class PageInfo
{
    public int totalResults { get; set; }
    public int resultsPerPage { get; set; }
}

public class Root
{
    public string? kind { get; set; }
    public string? etag { get; set; }
    public List<Item>? items { get; set; }
    public PageInfo? pageInfo { get; set; }
}

public class Status
{
    public string? uploadStatus { get; set; }
    public string? privacyStatus { get; set; }
    public string? license { get; set; }
    public bool embeddable { get; set; }
    public bool publicStatsViewable { get; set; }
    public bool madeForKids { get; set; }
}
