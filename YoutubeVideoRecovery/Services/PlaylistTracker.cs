using YoutubeVideoRecovery.Data;
using YoutubeVideoRecovery.Youtube.VideoStatus;
using YoutubeVideoRecovery.Youtube;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using YoutubeVideoRecovery.Models;

namespace YoutubeVideoRecovery.Services;

public class PlaylistTracker : IHostedService, IDisposable
{
    private readonly YoutubeRecoveryContext db;
    public PlaylistTracker(IServiceProvider serviceProvider)
    {
        db = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<YoutubeRecoveryContext>();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var hourInMs = 3600000;
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Playlist update Started !");
                var sw = new Stopwatch();
                sw.Start();
                var users = await db.Users.Where(u => u.ChannelId != null && u.ChannelId.Length > 0).ToListAsync();
                foreach (var user in users)
                {
                    Console.WriteLine($"Updating for user {user.Username}");
                    _ = Task.Run(() => UpdateUsersPlaylist(user),cancellationToken);
                }
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                Console.WriteLine(string.Format("Users Playlists updated. \n TimeTaken: {0:D2} {1:D2}h {2:D2}min {3:D2}sec {4:D3}ms", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds));
                await Task.Delay(6 * hourInMs, cancellationToken);
            }
        },cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task UpdateUsersPlaylist(User user)
    {
        if(user.ChannelId == null)
        {
            return;
        }
        var dbPlaylists = await db.Playlists.Where(p => p.UserId == user.UserId).ToListAsync();
        var apiPlaylists = await YoutubeApi.GetPlaylists(user);

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
        for (int i = 0; i < dbPlaylists.Count; i++)
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
            var apiVideos = await YoutubeApi.GetVideosInPlaylist(playlist.YtPlaylistId!, playlist.PlaylistId);

            var dbVideos = db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList();

            if (dbVideos.Count == 0)
            {
                db.Videos.AddRange(apiVideos);
                await db.SaveChangesAsync();
                dbVideos = new(db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList());
                continue;
            }

            var deletedVideos = dbVideos.Where(x => !apiVideos.Select(y => y.YtVideoId).ToList().Contains(x.YtVideoId)).ToList();
            var addedVideos = apiVideos.Where(x => !dbVideos.Select(y => y.YtVideoId).ToList().Contains(x.YtVideoId)).ToList();

            if (addedVideos.Count > 0)
            {
                db.Videos.AddRange(addedVideos);
                await db.SaveChangesAsync();
                dbVideos = new(db.Videos.Where(v => v.PlaylistId == playlist.PlaylistId).ToList());
            }

            var statusOfLostVideos = await YoutubeApi.GetVideoPrivacyStatus(string.Join(",", deletedVideos.Select(x => x.YtVideoId).ToList()));
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

            if (privateOrDeleted.Count > 0)
            {
                int videoRecovered = 0;
                foreach (var video in privateOrDeleted)
                {
                    var lostVideo = await db.Videos.FindAsync(video.VideoId);
                    if (lostVideo != null)
                    {
                        lostVideo.PrivacyStatus = "Private or Deleted";
                    }
                    await db.SaveChangesAsync();
                    videoRecovered++;
                }
                var userAcc = await db.Users.FindAsync(user.UserId);
                if(userAcc != null)
                {
                    userAcc.RecoveredVideos = videoRecovered;
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
            }
        }
    }
}
