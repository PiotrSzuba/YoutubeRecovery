using Newtonsoft.Json;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Google;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.Services;

namespace YoutubeVideoRecovery.Controllers;

public static class AccountController
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("api/account/{userId}", async (int userId,YoutubeRecoveryContext db) =>
        {
            var user = await db.Users.FindAsync(userId);
            if (user is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(user);
        });

        routes.MapPost("/api/account/login", async (GoogleToken token, YoutubeRecoveryContext db) =>
        {
            HttpClient client = new();
            var tokenInfo = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={token.access_token}");
            var profileInfo = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={token.access_token}");
            var content = profileInfo.Content.ReadAsStringAsync().Result;
            var googleUser = JsonConvert.DeserializeObject<GoogleUser>(content);

            User? dbUser = null;
            if (googleUser == null || googleUser.email == null)
            {
                return dbUser;
            }
            try
            {
                dbUser = db.Users.Where(x => x.Email == googleUser.email).First();
            }
            catch { }

            if (dbUser != null)
            {
                return dbUser;
            }

            dbUser = new User
            {
                Username = googleUser.name,
                Email = googleUser.email,
                PictureUrl = googleUser.picture,
                Locale = googleUser.locale,
                RecoveredVideos = 0,
            };
            db.Users.Add(dbUser);
            await db.SaveChangesAsync();

            return dbUser;
        });

        routes.MapPost("/api/account/addchannel", async (User inputUser, YoutubeRecoveryContext db, PlaylistTracker tracker) =>
        {
            var user = await db.Users.FindAsync(inputUser.UserId);
            if (user is null)
            {
                return Results.NotFound();
            }

            user.ChannelId = inputUser.ChannelId;
            await db.SaveChangesAsync();

            await tracker.UpdateUsersPlaylist(user);

            return Results.Ok(user);
        });

    }
}
