using Microsoft.EntityFrameworkCore;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.ViewModels;


namespace YoutubeVideoRecovery.Controllers;

public static class VideosController
{
    public static void MapVideosEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Videos", async (YoutubeRecoveryContext db) =>
        {
            return await db.Videos.ToListAsync();
        })
        .WithName("GetAllVideos");

        routes.MapGet("/api/Videos/{playlistId}", async(int PlaylistId, YoutubeRecoveryContext db) =>
        {
            var foundModel = await db.Playlists.FindAsync(PlaylistId);
            if (foundModel is null)
            {
                return Results.NotFound();
            }

            var test = await db.Videos.Where(v => v.PlaylistId == PlaylistId).Select(v => new VideoViewModel(v)).ToListAsync();
            test = test.OrderBy(x => x.Postition).ToList();
            return Results.Ok(test);
        })
        .WithName("GetVideosInPlayList");
    }
}

