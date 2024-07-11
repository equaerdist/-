using backend_iGamingBot.Infrastructure.Services;
using System.Runtime.CompilerServices;

namespace backend_iGamingBot.Infrastructure.Extensions
{
    public static class AppTests
    {
        public static async Task<WebApplication> TestOnYoutubeStreaming(this WebApplication app)
        {
            var youtubeNames = new string[] { "@LofiGirl", "@recrent", "@abc7NY", "@IlyaBodrovKrukowski", "@zeedthx" };
            using var scope = app.Services.CreateScope();
            var youtube = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var result = new List<(string, StreamInformation)>();
            foreach (var name in youtubeNames)
            {
                var id = await youtube.GetUserIdentifierByLinkAsync($"https://www.youtube.com/{name}");
                var streamResult = await youtube.UserIsStreaming(id);
                result.Add((name, streamResult));
            }
            throw new InvalidOperationException("Программа была запущена с тестами");
        }
    }
}
