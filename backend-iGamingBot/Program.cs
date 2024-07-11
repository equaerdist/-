using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Extensions;
using TwitchLib.Api;

var builder = WebApplication.CreateBuilder(args);

var cfg = builder.Configuration.Get<AppConfig>(o => o.BindNonPublicProperties = true) ??
    throw new InvalidProgramException();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInftrastructureServices();
builder.Services.AddAppServices(cfg);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//await app.TestOnYoutubeStreaming();
app.UseAuthorization();
TwitchAPI API = new TwitchAPI();

API.Settings.ClientId = cfg.TwitchClientId;
API.Settings.AccessToken = "d7c1gy18pqh8msu5u0zjyzmtanyd36";

var token = await API.Helix.Streams
    .GetStreamsAsync(userLogins: new() { "recrent" });
app.MapControllers();

app.Run();
