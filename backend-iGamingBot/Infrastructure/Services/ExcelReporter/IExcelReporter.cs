

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IExcelReporter
    {
        public Task<PostCreatorFile> GenerateExcel<T>(List<T> data, CancellationToken token = default);
    }
}
