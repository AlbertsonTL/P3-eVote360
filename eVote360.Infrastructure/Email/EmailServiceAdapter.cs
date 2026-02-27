using eVote360.Application.Abstractions.Emails;
using eVote360.Application.Abstractions.Services;

namespace eVote360.Infrastructure.Email;

/// <summary>Adapts IEmailSender (infra) to IEmailService (application)</summary>
public class EmailServiceAdapter : IEmailService
{
    private readonly IEmailSender _emailSender;

    public EmailServiceAdapter(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendEmailAsync(string to, string subject, string body)
        => _emailSender.SendEmailAsync(to, subject, body);

    public Task SendVotingConfirmationAsync(string to, string nombreCiudadano)
    {
        var subject = "Confirmación de Voto - eVote360";
        var body = BuildConfirmationEmail(nombreCiudadano);
        return _emailSender.SendEmailAsync(to, subject, body);
    }

    public Task SendVoteReceiptAsync(string to, string nombreCiudadano, string electionName, List<VoteReceiptItem> items)
    {
        var subject = $"Comprobante de Voto - {electionName} - eVote360";
        var body = BuildVoteReceiptEmail(nombreCiudadano, electionName, items);
        return _emailSender.SendEmailAsync(to, subject, body);
    }

    private static string BuildConfirmationEmail(string nombreCiudadano)
    {
        return $@"
<html><body style='font-family:Arial,sans-serif;background:#f4f4f4;padding:20px;'>
  <div style='max-width:600px;margin:auto;background:#fff;border-radius:8px;padding:30px;border-top:4px solid #0d6efd;'>
    <h2 style='color:#0d6efd;'><span>&#10003;</span> Su voto ha sido registrado</h2>
    <p>Estimado/a <strong>{nombreCiudadano}</strong>,</p>
    <p>Le confirmamos que su voto ha sido registrado exitosamente en el sistema de votación electrónica <strong>eVote360</strong>.</p>
    <hr>
    <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
    <p>Si usted no realizó esta acción, por favor comuníquese con las autoridades electorales de inmediato.</p>
    <hr>
    <p style='font-size:12px;color:#666;'>Este es un correo automático. No responda a este mensaje.</p>
    <p style='font-size:12px;color:#666;'>eVote360 — Sistema de Votación Electrónica</p>
  </div>
</body></html>";
    }

    private static string BuildVoteReceiptEmail(string nombreCiudadano, string electionName, List<VoteReceiptItem> items)
    {
        var rows = string.Join("", items.Select(item => $@"
            <tr>
                <td style='padding:10px;border:1px solid #dee2e6;'><strong>{item.PuestoNombre}</strong></td>
                <td style='padding:10px;border:1px solid #dee2e6;'>{item.CandidatoNombre}</td>
                <td style='padding:10px;border:1px solid #dee2e6;'>{item.PartidoNombre}</td>
            </tr>"));

        return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family:Arial,sans-serif;background:#f4f4f4;padding:20px;margin:0;'>
  <div style='max-width:640px;margin:auto;background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    
    <!-- Header -->
    <div style='background:#0d6efd;padding:24px 30px;'>
      <h1 style='color:#ffffff;margin:0;font-size:22px;'>&#128499; eVote360</h1>
      <p style='color:#cfe2ff;margin:4px 0 0;font-size:14px;'>Sistema de Votación Electrónica</p>
    </div>

    <!-- Body -->
    <div style='padding:30px;'>
      <h2 style='color:#0d6efd;margin-top:0;'>&#10003; Comprobante de Voto</h2>
      
      <p>Estimado/a <strong>{nombreCiudadano}</strong>,</p>
      <p>A continuación se detalla el resumen de su participación en:</p>
      <p style='font-size:18px;font-weight:bold;color:#333;padding:10px;background:#e9f0ff;border-radius:4px;'>
        &#128197; {electionName}
      </p>

      <h3 style='color:#333;border-bottom:2px solid #0d6efd;padding-bottom:8px;'>Detalle de sus votos</h3>
      
      <table style='width:100%;border-collapse:collapse;font-size:14px;'>
        <thead>
          <tr style='background:#0d6efd;color:#fff;'>
            <th style='padding:10px;text-align:left;border:1px solid #0d6efd;'>Puesto Electivo</th>
            <th style='padding:10px;text-align:left;border:1px solid #0d6efd;'>Candidato Elegido</th>
            <th style='padding:10px;text-align:left;border:1px solid #0d6efd;'>Partido</th>
          </tr>
        </thead>
        <tbody>
          {rows}
        </tbody>
      </table>

      <div style='margin-top:20px;padding:12px;background:#fff3cd;border-left:4px solid #ffc107;border-radius:4px;'>
        <strong>&#128274; Confidencialidad:</strong> Este correo es solo un comprobante de su participación. 
        El contenido de su voto es secreto e irrepetible.
      </div>

      <hr style='border:none;border-top:1px solid #dee2e6;margin:20px 0;'>
      <p><strong>Fecha y hora del voto:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
    </div>

    <!-- Footer -->
    <div style='background:#f8f9fa;padding:16px 30px;border-top:1px solid #dee2e6;'>
      <p style='font-size:12px;color:#6c757d;margin:0;'>
        Este es un correo automático generado por eVote360. No responda a este mensaje.<br>
        &copy; {DateTime.Now.Year} eVote360 — Sistema Electoral Electrónico
      </p>
    </div>
  </div>
</body>
</html>";
    }
}
