using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeVideoRecovery.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ChannelId { get; set; }
    public string? PictureUrl { get; set; }
    public string? Locale { get; set; }
    public int RecoveredVideos { get; set; }

    public virtual ICollection<Playlist>? Playlists { get; set; }
}
