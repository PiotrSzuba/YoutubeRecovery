using Microsoft.EntityFrameworkCore;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Models;
namespace YoutubeVideoRecovery.Controllers;

public static class PlaylistController
{
	public static void MapPlaylistEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Playlist", async (YoutubeRecoveryContext db) =>
        {
            return await db.Playlists.ToListAsync();
        })
        .WithName("GetAllPlaylists");

        routes.MapGet("/api/Playlist/{id}", async (int PlaylistId, YoutubeRecoveryContext db) =>
        {
            return await db.Playlists.FindAsync(PlaylistId)
                is Playlist model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetPlaylistById");
    }

}
