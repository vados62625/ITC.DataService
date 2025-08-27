namespace ITC.DataService.Interfaces;

public interface ICsvDataService
{
    Task<bool> UploadCsv(Stream fileStream, string? fileName);
}