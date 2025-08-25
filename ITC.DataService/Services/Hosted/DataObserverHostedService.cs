using ITC.DataService.Config;
using ITC.DataService.Interfaces;
using Microsoft.Extensions.Options;

namespace ITC.DataService.Services.Hosted;

public class DataObserverHostedService : IHostedService, IDisposable
{
    private readonly string _csvDirPath;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataObserverHostedService> _logger;
    private Timer? _timer;
    private FileSystemWatcher? _fileWatcher;
    private readonly HashSet<string> _processedFiles = new();
    private readonly TimeSpan _processingInterval;
    private readonly object _lock = new();

    public DataObserverHostedService(
        IOptions<DataObserverConfig> options, 
        IServiceProvider serviceProvider,
        ILogger<DataObserverHostedService> logger)
    {
        _csvDirPath = options.Value.CsvDirPath;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _processingInterval = TimeSpan.FromSeconds(options.Value.Interval);

        // Создаем директорию, если она не существует
        if (!Directory.Exists(_csvDirPath))
        {
            Directory.CreateDirectory(_csvDirPath);
            _logger.LogInformation("Создана директория: {DirectoryPath}", _csvDirPath);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DataObserverHostedService запущен");

        // Обработка существующих файлов при старте
        ProcessExistingFiles();

        // Настройка FileSystemWatcher для отслеживания новых файлов
        SetupFileWatcher();

        // Запуск таймера для периодической проверки
        _timer = new Timer(ProcessFiles, null, TimeSpan.Zero, _processingInterval);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DataObserverHostedService остановлен");

        _timer?.Change(Timeout.Infinite, 0);
        _fileWatcher?.Dispose();

        return Task.CompletedTask;
    }

    private void SetupFileWatcher()
    {
        _fileWatcher = new FileSystemWatcher(_csvDirPath)
        {
            Filter = "*.csv",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _fileWatcher.Created += OnFileCreated;
        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Error += OnFileWatcherError;
    }

    private void ProcessExistingFiles()
    {
        try
        {
            var csvFiles = Directory.GetFiles(_csvDirPath, "*.csv");
            _logger.LogInformation("Найдено {Count} CSV файлов при старте", csvFiles.Length);

            foreach (var filePath in csvFiles)
            {
                ProcessFile(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке существующих файлов");
        }
    }

    private void ProcessFiles(object state)
    {
        try
        {
            lock (_lock)
            {
                var csvFiles = Directory.GetFiles(_csvDirPath, "*.csv");
                
                foreach (var filePath in csvFiles)
                {
                    // Проверяем, не обработан ли уже файл
                    if (!_processedFiles.Contains(filePath))
                    {
                        ProcessFile(filePath);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при периодической обработке файлов");
        }
    }

    private void ProcessFile(string filePath)
    {
        try
        {
            // Проверяем, не заблокирован ли файл (еще пишется)
            if (IsFileLocked(filePath))
            {
                _logger.LogWarning("Файл {FileName} заблокирован, пропускаем", Path.GetFileName(filePath));
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var csvDataService = scope.ServiceProvider.GetRequiredService<ICsvDataService>();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            csvDataService.UploadCsv(fileStream);
            _logger.LogInformation("Файл {FileName} успешно обработан", Path.GetFileName(filePath));
                
            // Добавляем файл в список обработанных
            lock (_lock)
            {
                _processedFiles.Add(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке файла {FileName}", Path.GetFileName(filePath));
        }
    }

    private bool IsFileLocked(string filePath)
    {
        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("Обнаружен новый файл: {FileName}", e.Name);
        ProcessFile(e.FullPath);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Обрабатываем изменения файлов (если нужно отслеживать изменения существующих файлов)
        _logger.LogInformation("Файл изменен: {FileName}", e.Name);
        
        // Убираем из обработанных, чтобы обработать снова
        lock (_lock)
        {
            _processedFiles.Remove(e.FullPath);
        }
    }

    private void OnFileWatcherError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "Ошибка FileSystemWatcher");
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _fileWatcher?.Dispose();
    }
}