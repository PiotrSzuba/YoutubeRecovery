using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Youtube;
using YoutubeVideoRecovery.Youtube.Playlist;
using YoutubeVideoRecovery.Youtube.Video;
using YoutubeVideoRecovery.Youtube.VideoStatus;
using YoutubeVideoRecovery.Controllers;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Sockets;
using System.Net;
using YoutubeVideoRecovery.Models;
using YoutubeVideoRecovery.Utils;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using YoutubeVideoRecovery.Google;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

var configuration = builder.Configuration;

builder.Services.AddDbContext<YoutubeRecoveryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YoutubeRecoveryContext")));

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddHostedService<YoutubeVideoRecovery.Services.PlaylistTracker>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .WithOrigins("http://localhost:3000")
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", () =>
{
    return "Hello Youtube";
});

app.MapPost("/api/login", async (GoogleToken token, YoutubeRecoveryContext db) =>
{
    HttpClient client = new ();
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
    catch{}

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

app.MapPost("/api/account",async (User inputUser,YoutubeRecoveryContext db) =>
{
    var user = await db.Users.FindAsync(inputUser.UserId);
    if (user is null)
    {
        return Results.NotFound();
    }

    user.ChannelId = inputUser.ChannelId;
    await db.SaveChangesAsync();

    return Results.Ok(user);

    var apiPlaylists = await YoutubeApi.GetPlaylists(user);
    db.Playlists.AddRange(apiPlaylists);
    await db.SaveChangesAsync();
    foreach(var playlist in apiPlaylists)
    {
        var apiVideos = await YoutubeApi.GetVideosInPlaylist(playlist.YtPlaylistId!, playlist.PlaylistId);
    }

    return Results.Ok(user);
});


app.MapPlaylistEndpoints();
app.MapVideosEndpoints();

app.Run();