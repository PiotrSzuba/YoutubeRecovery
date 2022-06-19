using System.ComponentModel.DataAnnotations.Schema;
using YoutubeVideoRecovery.Models;
using System.Text.Json.Serialization;

namespace YoutubeVideoRecovery.ViewModels;

public class VideoViewModel
{
    public int VideoId { get; set; }
    public int PlaylistId { get; set; }
    public string? YtVideoId { get; set; }
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int Postition { get; set; }
    public string? VideoOwnerChannelTitle { get; set; }
    public string? VideoOwnerChannelId { get; set; }
    public string? PrivacyStatus { get; set; }

    [JsonConstructor]
    public VideoViewModel() { }
    public VideoViewModel(Video video)
    {
        VideoId = video.VideoId;
        PlaylistId = video.PlaylistId;
        YtVideoId = video.YtVideoId;
        Title = video.Title;
        ThumbnailUrl = video.ThumbnailUrl;
        Postition = video.Postition;
        VideoOwnerChannelTitle = video.VideoOwnerChannelTitle;
        VideoOwnerChannelId = video.VideoOwnerChannelId;
        PrivacyStatus = video.PrivacyStatus;
    }
}
