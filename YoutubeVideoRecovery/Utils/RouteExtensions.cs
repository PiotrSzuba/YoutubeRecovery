using Microsoft.AspNetCore.Builder;

namespace YoutubeVideoRecovery.Utils;

public static class RouteExtensions
{
    public static int Authorize(HttpRequest request)
    {
        var userID = request.Headers["user"].ToString();
        if (userID == null)
        {
            return 0;
        }
        var parsed = int.TryParse(userID, out int userId);
        if (userId == 0 || !parsed)
        {
            return 0;
        }

        return userId;
    }
}
