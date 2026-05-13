public interface IFileStorageService
{
    Task<string> SavePhotoAsync(int memberId, IFormFile file);
    void DeletePhoto(string? relativePath);
}

public class FileStorageService : IFileStorageService
{
    private readonly string _photosFolder;

    public FileStorageService()
    {
        // Папка wwwroot/photos в корне бэкенда
        _photosFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
    }

    public async Task<string> SavePhotoAsync(int memberId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран или пуст");

        if (!Directory.Exists(_photosFolder))
        {
            Directory.CreateDirectory(_photosFolder);
        }

        // Генерируем уникальное имя файла
        var fileName = $"{memberId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_photosFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Возвращаем относительный путь для хранения в БД
        return $"photos/{fileName}";
    }

    public void DeletePhoto(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (Exception ex)
        {
            // Логируем ошибку, но не крашим приложение
            Console.WriteLine($"Ошибка при удалении файла {relativePath}: {ex.Message}");
        }
    }
}
