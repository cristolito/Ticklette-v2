using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Ticklette.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly IConfiguration _configuration;

    public CloudinaryService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var cloudinaryConfig = _configuration.GetSection("Cloudinary");
        var cloudName = cloudinaryConfig["CloudName"];
        var apiKey = cloudinaryConfig["ApiKey"];
        var apiSecret = cloudinaryConfig["ApiSecret"];

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary configuration is missing in appsettings.json");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account)
        {
            Api = { Secure = true }
        };
    }

    // ✅ Subir imagen desde IFormFile
    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // Validar que sea una imagen
        if (!file.ContentType.StartsWith("image/"))
            throw new ArgumentException("File is not an image");

        // Validar tamaño (máximo 5MB)
        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("File size exceeds 5MB");

        await using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"ticklette/events/{subFolder}" // Organizar en carpeta
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    // ✅ Eliminar imagen de Cloudinary
    public async Task<DeletionResult> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }

    // ✅ Obtener URL de imagen optimizada
    public string GetOptimizedImageUrl(string publicId, int width = 800, int height = 600)
    {
        return _cloudinary.Api.UrlImgUp
            .Transform(new Transformation()
                .Width(width)
                .Height(height)
                .Crop("fill")
                .Gravity("auto")
                .Quality("auto"))
            .BuildUrl(publicId);
    }

    // ✅ Extraer publicId de la URL de Cloudinary
    public string ExtractPublicIdFromUrl(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var segments = uri.Segments;
        // La estructura típica es: /v1234567890/folder/image.jpg
        if (segments.Length >= 3)
        {
            var publicIdWithExtension = segments[^1]; // último segmento
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);
            return publicId;
        }
        return string.Empty;
    }
}