
using OfficeOpenXml;
using System.Reflection;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class ExcelReporter : IExcelReporter
    {
        public async Task<PostCreatorFile> GenerateExcel<T>(List<T> data, CancellationToken t = default)
        {
            return await Task.Run(() =>
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    var properties = typeof(T).GetProperties();
                    for (int col = 1; col <= properties.Length; col++)
                    {
                        var property = properties[col - 1];
                        var attribute = property.GetCustomAttribute<ColumnLabelAttribute>();
                        var columnName = attribute?.ColumnName ?? property.Name;
                        worksheet.Cells[1, col].Value = columnName;
                    }
                    for (int row = 2; row <= data.Count + 1; row++)
                    {
                        var item = data[row - 2];
                        for (int col = 1; col <= properties.Length; col++)
                        {
                            var property = properties[col - 1];
                            var value = property.GetValue(item);
                            worksheet.Cells[row, col].Value = value;
                        }
                    }


                    var stream = new MemoryStream();
                    
                    package.SaveAs(stream);
                    stream.Position = 0;

                    return new PostCreatorFile() { Stream = stream, Name = "GeneratedExcel.xlsx" };
                }
            }, t);
           
        }
    }
}
