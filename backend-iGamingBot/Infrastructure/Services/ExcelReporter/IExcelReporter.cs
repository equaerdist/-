namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IExcelReporter
    {
        public Task<IFormFile> GenerateExcel<T>(List<T> data, CancellationToken token = default);
    }
}
