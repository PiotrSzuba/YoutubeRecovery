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
using YoutubeVideoRecovery.Services;

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

builder.Services.AddSingleton<PlaylistTracker>();
builder.Services.AddHostedService(provider => provider.GetService<PlaylistTracker>()!);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

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
    return "Hello API consumer";
});

app.MapPlaylistEndpoints();
app.MapVideosEndpoints();
app.MapAccountEndpoints();

app.Run();