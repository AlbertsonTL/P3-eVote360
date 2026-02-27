namespace eVote360.Application.Common.Orc;

public class TesseractOcrMock : IOcrService
{
    // El mock nunca está "disponible" como OCR real,
    // así que el controlador lo tratará como no operativo y aprobará sin OCR.
    public bool IsAvailable => false;

    public Task<string> ExtractTextFromImageAsync(string imagePath)
    {
        return Task.FromResult(string.Empty);
    }
}
