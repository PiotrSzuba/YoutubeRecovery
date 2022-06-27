using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeVideoRecovery.Models;

public class Playlist
{
    [Key]
    public int PlaylistId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public string? YtPlaylistId { get; set; }
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    
    public virtual ICollection<Video>? Videos { get; set; }
}
