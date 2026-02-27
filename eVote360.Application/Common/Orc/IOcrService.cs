namespace eVote360.Application.Common.Orc;

public interface IOcrService
{
    /// <summary>
    /// Indica si el motor OCR está disponible y operativo.
    /// Si retorna false, la validación de identidad debe aprobarse sin OCR.
    /// </summary>
    bool IsAvailable { get; }

    Task<string> ExtractTextFromImageAsync(string imagePath);
}
