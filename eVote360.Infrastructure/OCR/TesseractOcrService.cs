using eVote360.Application.Common.Orc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace eVote360.Infrastructure.OCR;

/// <summary>
/// Implementación real del servicio OCR usando Tesseract.
/// Extrae el texto de una imagen de cédula y devuelve todo el texto encontrado.
/// Si tessdata no está instalado, IsAvailable = false y el controlador aprueba
/// la validación directamente (modo desarrollo/demo).
/// </summary>
public class TesseractOcrService : IOcrService
{
    private readonly string _tessDataPath;
    private readonly ILogger<TesseractOcrService> _logger;

    public TesseractOcrService(IConfiguration configuration, ILogger<TesseractOcrService> logger)
    {
        _logger = logger;
        _tessDataPath = configuration["Ocr:TessDataPath"]
                        ?? Path.Combine(AppContext.BaseDirectory, "tessdata");
    }

    /// <summary>
    /// true si la carpeta tessdata existe y contiene el archivo de idioma español.
    /// </summary>
    public bool IsAvailable =>
        Directory.Exists(_tessDataPath) &&
        File.Exists(Path.Combine(_tessDataPath, "spa.traineddata"));

    public async Task<string> ExtractTextFromImageAsync(string imagePath)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default);
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);

                var text = page.GetText();
                _logger.LogInformation("OCR extrajo texto (longitud {Len}): {Text}", text.Length, text);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar OCR para imagen '{Path}'", imagePath);
                return string.Empty;
            }
        });
    }
}
