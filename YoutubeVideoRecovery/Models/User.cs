using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeVideoRecovery.Models;

public class User
{
    [Key]
    public int UserId { get; set; }


    //public virtual ICollection<Playlist>? Playlists { get; set; }
}
