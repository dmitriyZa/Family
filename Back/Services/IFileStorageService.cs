public interface IFileStorageService
{
    Task<string> SavePhotoAsync(int memberId, IFormFile file);
    void DeletePhoto(string? relativePath);
}



public class FileStorageService : IFileStorageService
{
    private readonly string _photosFolder;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _logger = logger;
        var basePath = configuration["FileStorage:BasePath"] ?? "/data";
        _photosFolder = Path.Combine(basePath, "photos");

        if (!Directory.Exists(_photosFolder))
        {
            Directory.CreateDirectory(_photosFolder);
            _logger.LogInformation("Создана папка для хранения фото: {PhotosFolder}", _photosFolder);
        }
    }

    public async Task<string> SavePhotoAsync(int memberId, IFormFile file)
    {
        ValidateFile(file);

        var fileName = $"{memberId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_photosFolder, fileName);

        try
        {
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            _logger.LogInformation("Фото сохранено: {FilePath}", filePath);
            return $"photos/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении фото для memberId {MemberId}", memberId);
            throw;
        }
    }

    public void DeletePhoto(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        try
        {
            var fullPath = Path.Combine(_photosFolder, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Файл удалён: {FullPath}", fullPath);
            }
            else
            {
                _logger.LogWarning("Попытка удаления несуществующего файла: {RelativePath}", relativePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении файла {RelativePath}", relativePath);
        }
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран или пуст");

        const long maxSize = 10 * 1024 * 1024;
        if (file.Length > maxSize)
            throw new ArgumentException($"Файл слишком большой. Максимальный размер: {maxSize / 1024 / 1024} МБ");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Недопустимый формат файла. Разрешены: JPG, PNG, GIF, BMP");
    }
}

