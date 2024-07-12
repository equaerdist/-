using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Extensions;
using TwitchLib.Api;

var builder = WebApplication.CreateBuilder(args);

var cfg = builder.Configuration.Get<AppConfig>(o => o.BindNonPublicProperties = true) ??
    throw new InvalidProgramException();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInftrastructureServices(cfg);
builder.Services.AddAppServices(cfg);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//await app.TestOnYoutubeStreaming();
//await app.CreateFakeStreamers();
await app.CheckMigrationsAsync();
app.UseAuthorization();
app.MapControllers();

app.Run();
