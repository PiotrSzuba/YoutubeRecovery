using Microsoft.EntityFrameworkCore;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.Utils;
using YoutubeVideoRecovery.ViewModels;

namespace YoutubeVideoRecovery.Controllers;

public static class VideosController
{
    public static void MapVideosEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Videos/{playlistId}", async(int PlaylistId, YoutubeRecoveryContext db, HttpRequest request) =>
        {
            var auth = RouteExtensions.Authorize(request);
            if (auth == 0)
            {
                return Results.Unauthorized();
            }

            var model = await db.Playlists.FindAsync(PlaylistId);
            if (model is null)
            {
                return Results.NotFound();
            }

            if (model.UserId != auth)
            {
                return Results.Unauthorized();
            }

            var videos = await db.Videos.Where(v => v.PlaylistId == PlaylistId).Select(v => new VideoViewModel(v)).ToListAsync();
            videos = videos.OrderBy(x => x.Postition).ToList();
            return Results.Ok(videos);
        })
        .WithName("GetVideosInPlayList");
    }
}

