using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ITC.DataService.Interfaces;
using Newtonsoft.Json;

namespace ITC.DataService.Services;

public class CsvService : ICsvService
{
    private readonly ILogger<CsvService> _logger;

    public CsvService(ILogger<CsvService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessCsvToJsonAsync(Stream csvStream)
    {
        var columnsData = await GetColumnsData(csvStream);
        var jsonData = JsonConvert.SerializeObject(columnsData);
        return jsonData;
    }

    public async Task<IDictionary<string, IList<string>>> GetColumnsData(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 });
        
        await csv.ReadAsync();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;
        if (headers == null) throw new Exception("Headers not found");

        var columnsData = new Dictionary<string, IList<string>>();
        foreach (var header in headers)
        {
            columnsData[header] = new List<string>();
        }

        while (await csv.ReadAsync())
        {
            foreach (var header in headers)
            {
                var data = csv.GetField(header);
                if (!string.IsNullOrEmpty(data))
                    columnsData[header].Add(data);
            }
        }
        
        return columnsData;
    }
    
    public IEnumerable<IList<string>> GetRowsData<T>(Stream csvStream) where T : class
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
        { 
            Delimiter = ";", 
            Encoding = Encoding.UTF8 
        });
    
        var data = csv.GetRecords<object>().ToList();

        return data.Select(c =>
        {
            dynamic dynamic = c;
            var list = new List<string>
            {
                dynamic.current_R,
                dynamic.current_S,
                dynamic.current_T
            };
            return list;
        });
    }
}