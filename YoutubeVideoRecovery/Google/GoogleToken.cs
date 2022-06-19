namespace YoutubeVideoRecovery.Google;

public class GoogleToken
{
    public string? access_token { get; set; }
    public string? token_type { get; set; }
    public int expires_in { get; set; }
    public string? scope { get; set; }
    public string? authuser { get; set; }
    public string? hd { get; set; }
    public string? prompt { get; set; }
}
