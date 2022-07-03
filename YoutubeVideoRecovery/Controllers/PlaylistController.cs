using Microsoft.EntityFrameworkCore;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.Utils;
using System.Text.Json;

namespace YoutubeVideoRecovery.Controllers;

public static class PlaylistController
{
	public static void MapPlaylistEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Playlist", async (YoutubeRecoveryContext db, HttpRequest request) =>
        {
            var auth = RouteExtensions.Authorize(request);
            if (auth == 0)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(await db.Playlists.Where(p => p.UserId == auth).ToListAsync());
        })
        .WithName("GetAllPlaylists");

        routes.MapGet("/api/Playlist/{id}", async (int PlaylistId, YoutubeRecoveryContext db, HttpRequest request) =>
        {
            var auth = RouteExtensions.Authorize(request);
            if (auth == 0)
            {
                return Results.Unauthorized();
            }

            var model = await db.Playlists.FindAsync(PlaylistId);
            if(model == null)
            {
                return Results.NotFound();
            }

            if (model.UserId != auth)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(model);
        })
        .WithName("GetPlaylistById");
    }

}
