using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace YoutubeVideoRecovery.Models;

public class Video
{
    [Key]
    public int VideoId { get; set; }
    [ForeignKey("Playlist")]
    public int PlaylistId { get; set; }
    public virtual Playlist? Playlist { get; set; }
    public string? YtVideoId { get; set; }
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int Postition { get; set; }
    public string? VideoOwnerChannelTitle { get; set; }
    public string? VideoOwnerChannelId { get; set; }
    public string? PrivacyStatus { get; set; }

}
