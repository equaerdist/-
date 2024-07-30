using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Extensions;
using backend_iGamingBot.Infrastructure.Services;
using Npgsql;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

var cfg = builder.Configuration.Get<AppConfig>(o => o.BindNonPublicProperties = true) ??
    throw new InvalidProgramException();
AppConfig.GlobalInstance = cfg;
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInftrastructureServices(cfg);
builder.Services.AddAppServices(cfg);
var app = builder.Build();
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

if (app.Environment.IsDevelopment())
{
    //await app.TestOnYoutubeStreaming();
    //await app.CreateFakeStreamersWithSubscribers();
    //await app.CreateInfoForStreamersPage();
    //await app.TestFunction();
    //await app.CreateTestRaffleForUser();
    //await app.CreateTestUser();
    //await app.CreateTestYoutubeChannel();
    //await app.StreamersTest();
    //await app.PostTestWithFile();
    //app.TestYtIdFinder();
    //await app.TestStreamerInvites();
    //await app.TestAdminInvites();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.CheckMigrationsAsync();
app.UseMiddleware<ExceptionHandler>();
app.UseCors(opt => opt
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(cfg.Frontend)
    .AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
