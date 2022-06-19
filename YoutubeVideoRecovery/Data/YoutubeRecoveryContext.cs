#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YoutubeVideoRecovery.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace YoutubeVideoRecovery.Data;

public class YoutubeRecoveryContext : DbContext
{
    public YoutubeRecoveryContext(DbContextOptions<YoutubeRecoveryContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Playlist> Playlists { get; set; }

}
