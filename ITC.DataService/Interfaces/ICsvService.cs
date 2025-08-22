namespace ITC.DataService.Interfaces;

public interface ICsvService
{
    Task<string> ProcessCsvToJsonAsync(Stream csvStream);
    Task<IDictionary<string, IList<string>>> GetColumnsData(Stream csvStream);
    IEnumerable<IList<string>> GetRowsData<T>(Stream csvStream) where T : class;
}