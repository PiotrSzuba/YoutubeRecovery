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
using NuGet.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var configuration = builder.Configuration;

builder.Services.AddDbContext<YoutubeRecoveryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YoutubeRecoveryContext")));

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });

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

app.MapGet("/html", () =>
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    return Results.Extensions.Html(File.ReadAllText(path + @"\index.html"));
});

app.MapPost("/api/login", async (GoogleToken token, YoutubeRecoveryContext db) =>
{
    HttpClient client = new HttpClient();
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

app.MapGet("/", async (YoutubeRecoveryContext db) =>
{
    var dbPlaylists = await db.Playlists.ToListAsync();
    var apiPlaylists = await YoutubeApi.GetPlaylists();

    string returnString = ""; ////////////

    if (dbPlaylists.Count == 0)
    {
        db.Playlists.AddRange(apiPlaylists);
        await db.SaveChangesAsync();
        dbPlaylists = new(await db.Playlists.ToListAsync());
    }

    var deletedPlaylists = dbPlaylists.Where(x => !apiPlaylists.Select(y => y.YtPlaylistId).ToList().Contains(x.YtPlaylistId)).ToList();
    var addedPlaylists = apiPlaylists.Where(x => !dbPlaylists.Select(y => y.YtPlaylistId).ToList().Contains(x.YtPlaylistId)).ToList();

    if (addedPlaylists.Count > 0)
    {
        db.Playlists.AddRange(addedPlaylists);
        await db.SaveChangesAsync();
        dbPlaylists = new(await db.Playlists.ToListAsync());
    }

    if (deletedPlaylists.Count > 0)
    {
        db.Playlists.RemoveRange(deletedPlaylists);
        await db.SaveChangesAsync();
        dbPlaylists = new(await db.Playlists.ToListAsync());
    }

    bool breakFlag = false;
    for (int i = 0; i  < dbPlaylists.Count; i++)
    {
        for (int j = 0; j < apiPlaylists.Count; j++)
        {
            breakFlag = false;
            if (dbPlaylists[i].YtPlaylistId != apiPlaylists[j].YtPlaylistId)
            {
                continue;
            }
            if (dbPlaylists[i].Title == apiPlaylists[j].Title)
            {
                breakFlag = true;
            }
            if (dbPlaylists[i].ThumbnailUrl != apiPlaylists[j].ThumbnailUrl)
            {
                breakFlag = false;
            }
            if (breakFlag)
            {
                break;
            }
            var playlist = await db.Playlists.FindAsync(dbPlaylists[i].PlaylistId);
            if (playlist is null)
            {
                break;
            }
            playlist.Title = apiPlaylists[j].Title;
            playlist.ThumbnailUrl = apiPlaylists[j].ThumbnailUrl;
            await db.SaveChangesAsync();
        }
    }

    foreach (var playlist in dbPlaylists)
    {
        returnString += "\n" + playlist.Title + " Id: " + playlist.YtPlaylistId + " " + playlist.ThumbnailUrl; ///////////////

        var apiVideos = await YoutubeApi.GetVideosInPlaylist(playlist.YtPlaylistId!, playlist.PlaylistId);

        var dbVideos = db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList();

        if (dbVideos.Count == 0)
        {
            db.Videos.AddRange(apiVideos);
            await db.SaveChangesAsync();
            dbVideos = new(db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList());
        }

        var deletedVideos = dbVideos.Where(x => !apiVideos.Select(y => y.YtVideoId).ToList().Contains(x.YtVideoId)).ToList();
        var addedVideos = apiVideos.Where(x => !dbVideos.Select(y => y.YtVideoId).ToList().Contains(x.YtVideoId)).ToList();

        if (addedVideos.Count > 0)
        {
            db.Videos.AddRange(addedVideos);
            await db.SaveChangesAsync();
            dbVideos = new(db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList());
        }

        var statusOfLostVideos = await YoutubeApi.GetVideoPrivacyStatus(String.Join(",", deletedVideos.Select(x => x.YtVideoId).ToList()));
        var lostVideosStatus = YoutubeVideoStatus.GetObject(statusOfLostVideos);
        var deletedByUser = new List<string>();
        if (lostVideosStatus.root != null && lostVideosStatus.root.items != null)
        {
            deletedByUser.AddRange(lostVideosStatus.root.items.Select(x => x.id!));
        }
        var privateOrDeleted = deletedVideos.Where(x => !deletedByUser.Contains(x.YtVideoId)).ToList();
        deletedVideos = deletedVideos.Where(x => deletedByUser.Contains(x.YtVideoId)).ToList();


        if (deletedVideos.Count > 0)
        {
            db.Videos.RemoveRange(deletedVideos);
            await db.SaveChangesAsync();
            dbVideos = new(db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList());
        }

        if(privateOrDeleted.Count > 0)
        {
            foreach (var video in privateOrDeleted)
            {
                var lostVideo = await db.Videos.FindAsync(video.VideoId);
                if (lostVideo != null)
                {
                    lostVideo.PrivacyStatus = "Private or Deleted";
                }
                await db.SaveChangesAsync();
            }
        }

        breakFlag = false;
        for (int i = 0; i < dbVideos.Count; i++)
        {
            for (int j = 0; j < apiVideos.Count; j++)
            {
                breakFlag = false;
                if (dbVideos[i].YtVideoId != apiVideos[j].YtVideoId)
                {
                    continue;
                }
                if (dbVideos[i].Title == apiVideos[j].Title)
                {
                    breakFlag = true;
                }
                if (dbVideos[i].ThumbnailUrl != apiVideos[j].ThumbnailUrl)
                {
                    breakFlag = false;
                }
                if (dbVideos[i].PrivacyStatus != apiVideos[j].PrivacyStatus)
                {
                    breakFlag = false;
                }
                if (dbVideos[i].Postition != apiVideos[j].Postition)
                {
                    breakFlag = false;
                }
                if (breakFlag)
                {
                    break;
                }
                var video = await db.Videos.FindAsync(dbVideos[i].VideoId);
                if (video is null)
                {
                    break;
                }
                video.Title = apiVideos[j].Title;
                video.ThumbnailUrl = apiVideos[j].ThumbnailUrl;
                video.PrivacyStatus = apiVideos[j].PrivacyStatus;
                video.Postition = apiVideos[j].Postition;
                video.VideoOwnerChannelTitle = apiVideos[j].VideoOwnerChannelTitle;

                await db.SaveChangesAsync();
            }
        }

        foreach (var video in dbVideos)
        {
            if (video.YtVideoId == null)
            {
                continue;
            }

            returnString += $"\n\t {video.Postition} {video.Title} {video.YtVideoId} {video.PrivacyStatus}";
        }
    }

    return returnString;

});

app.MapPlaylistEndpoints();
app.MapVideosEndpoints();

app.Run();