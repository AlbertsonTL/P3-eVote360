using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Common.Orc;
using eVote360.Application.DTOs.Request;
using eVote360.Web.Models;

namespace eVote360.Web.Controllers;

public class ElectorController : Controller
{
    private readonly ICitizenRepository _citizenRepository;
    private readonly IElectionService _electionService;
    private readonly IVotacionService _votacionService;
    private readonly IPositionService _positionService;
    private readonly ICandidateService _candidateService;
    private readonly IVoteRepository _voteRepository;
    private readonly IOcrService _ocrService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ElectorController> _logger;

    public ElectorController(
        ICitizenRepository citizenRepository,
        IElectionService electionService,
        IVotacionService votacionService,
        IPositionService positionService,
        ICandidateService candidateService,
        IVoteRepository voteRepository,
        IOcrService ocrService,
        IEmailService emailService,
        ILogger<ElectorController> logger)
    {
        _citizenRepository = citizenRepository;
        _electionService = electionService;
        _votacionService = votacionService;
        _positionService = positionService;
        _candidateService = candidateService;
        _voteRepository = voteRepository;
        _ocrService = ocrService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Home");

    [HttpGet]
    public IActionResult ValidarElector() => RedirectToAction("Index", "Home");

    [HttpPost]
    public async Task<IActionResult> ValidarElector(string numeroDocumento)
    {
        var electionResult = await _electionService.GetActiveElectionAsync();
        if (!electionResult.IsSuccess)
        {
            ViewBag.Message = "No hay ningún proceso electoral activo en estos momentos.";
            ViewBag.TipoMensaje = "warning";
            return View("Mensaje");
        }

        var citizen = await _citizenRepository.GetByNationalIdAsync(numeroDocumento);
        if (citizen == null)
        {
            ViewBag.Message = "Ciudadano no encontrado. Verifique el número de documento ingresado.";
            ViewBag.TipoMensaje = "error";
            return View("Mensaje");
        }

        if (!citizen.IsActive)
        {
            ViewBag.Message = "Su cuenta de ciudadano está inactiva. Contacte al administrador.";
            ViewBag.TipoMensaje = "warning";
            return View("Mensaje");
        }

        var hasVoted = await _votacionService.HasVotedAsync(citizen.Id, electionResult.Data!.Id);
        if (hasVoted.Data)
        {
            ViewBag.Message = "Ya ha ejercido su derecho al voto en esta elección.";
            ViewBag.TipoMensaje = "info";
            return View("Mensaje");
        }

        HttpContext.Session.SetInt32("CitizenId", citizen.Id);
        HttpContext.Session.SetString("NumeroDocumento", numeroDocumento);
        HttpContext.Session.SetInt32("ElectionId", electionResult.Data!.Id);
        HttpContext.Session.SetString("CitizenEmail", citizen.Email.Value);
        HttpContext.Session.SetString("CitizenNombre", $"{citizen.Nombre} {citizen.Apellido}");

        return RedirectToAction("ValidarIdentidad");
    }

    [HttpGet]
    public IActionResult ValidarIdentidad()
    {
        var documento = HttpContext.Session.GetString("NumeroDocumento");
        if (string.IsNullOrEmpty(documento))
            return RedirectToAction("Index", "Home");
        var model = new ElectorViewModels.ValidarIdentidadVM { NumeroDocumento = documento };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ValidarIdentidad(ElectorViewModels.ValidarIdentidadVM model)
    {
        var cedula = model.FotoCedula;
        if (cedula == null || cedula.Length == 0)
        {
            ViewBag.Error = "Debe subir una foto de su cédula.";
            return View(model);
        }

        var originalDocumento = HttpContext.Session.GetString("NumeroDocumento") ?? "";

        if (!_ocrService.IsAvailable)
        {
            return RedirectToAction("Opciones");
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(cedula.FileName)}";
        var filePath = Path.Combine(Path.GetTempPath(), fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await cedula.CopyToAsync(stream);
        }

        var extractedText = await _ocrService.ExtractTextFromImageAsync(filePath);

        var docLimpio = originalDocumento.Replace("-", "").Trim();
        var textLimpio = extractedText.Replace("-", "").Replace(" ", "");

        if (!textLimpio.Contains(docLimpio) && !extractedText.Contains(originalDocumento))
        {
            ViewBag.Error = "Los datos extraídos de la foto no coinciden con su documento. Por favor intente con una foto más clara.";
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            model.NumeroDocumento = originalDocumento;
            return View(model);
        }

        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        return RedirectToAction("Opciones");
    }

    [HttpGet]
    public async Task<IActionResult> Opciones()
    {
        var citizenId = HttpContext.Session.GetInt32("CitizenId");
        var electionId = HttpContext.Session.GetInt32("ElectionId");
        var numeroDocumento = HttpContext.Session.GetString("NumeroDocumento");

        if (citizenId == null || electionId == null)
            return RedirectToAction("Index", "Home");

        var positionsResult = await _positionService.GetAllAsync();
        var activePositions = positionsResult.Data?.Where(p => p.IsActive).ToList() ?? new();

        var votedPositions = await _voteRepository.GetVotedPositionsAsync(citizenId.Value, electionId.Value);

        var allCandidatesResult = await _candidateService.GetAllAsync();

        var puestosVM = new List<ElectorViewModels.PuestoDisponibleVM>();
        foreach (var pos in activePositions)
        {
            var candidatesForPos = allCandidatesResult.Data?.Where(c => c.PositionId == pos.Id && c.IsActive).Count() ?? 0;
            var partiesForPos = allCandidatesResult.Data?.Where(c => c.PositionId == pos.Id && c.IsActive)
                .Select(c => c.PartyId).Distinct().Count() ?? 0;

            puestosVM.Add(new ElectorViewModels.PuestoDisponibleVM
            {
                PuestoId = pos.Id,
                NombrePuesto = pos.Nombre,
                CantidadCandidatos = candidatesForPos,
                CantidadPartidos = partiesForPos,
                YaVoto = votedPositions.Contains(pos.Id)
            });
        }

        // Verificar si falta votar algún puesto
        if (TempData.ContainsKey("PuestosFaltantes"))
            ViewBag.PuestosFaltantes = TempData["PuestosFaltantes"];

        var model = new ElectorViewModels.OpcionesPuestosVM
        {
            NumeroDocumento = numeroDocumento ?? "",
            Puestos = puestosVM
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Boleta(int puestoId, string numeroDocumento)
    {
        var citizenId = HttpContext.Session.GetInt32("CitizenId");
        if (citizenId == null)
            return RedirectToAction("Index", "Home");

        var allCandidatesResult = await _candidateService.GetAllAsync();
        var candidatesForPos = allCandidatesResult.Data?
            .Where(c => c.PositionId == puestoId && c.IsActive)
            .ToList() ?? new();

        var positionsResult = await _positionService.GetAllAsync();
        var position = positionsResult.Data?.FirstOrDefault(p => p.Id == puestoId);

        var candidatosVM = candidatesForPos.Select(c => new ElectorViewModels.CandidatoBoletaVM
        {
            CandidatoId = c.Id,
            NombreCandidato = c.Nombre,
            ApellidoCandidato = c.Apellido,
            FotoCandidato = string.IsNullOrEmpty(c.FotoPath) ? "/images/candidates/default.png" : c.FotoPath,
            LogoPartido = "/images/logos/default.png",
            SiglasPartido = c.PartyNombre,
            NombrePartido = c.PartyNombre
        }).ToList();

        var model = new ElectorViewModels.BoletaVM
        {
            PuestoId = puestoId,
            NombrePuesto = position?.Nombre ?? "Puesto",
            NumeroDocumento = numeroDocumento,
            Candidatos = candidatosVM
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarVoto(int puestoId, string numeroDocumento, int? candidatoSeleccionadoId)
    {
        var citizenId = HttpContext.Session.GetInt32("CitizenId");
        var electionId = HttpContext.Session.GetInt32("ElectionId");

        if (citizenId == null || electionId == null)
            return RedirectToAction("Index", "Home");

        if (candidatoSeleccionadoId == null)
        {
            TempData["Error"] = "Debe seleccionar un candidato antes de votar.";
            return RedirectToAction("Boleta", new { puestoId, numeroDocumento });
        }

        // Determinar el partyId del candidato seleccionado
        int? partyId = null;
        if (candidatoSeleccionadoId > 0)
        {
            var candidatoResult = await _candidateService.GetByIdAsync(candidatoSeleccionadoId.Value);
            if (candidatoResult.IsSuccess && candidatoResult.Data != null)
                partyId = candidatoResult.Data.PartyId;
        }

        var dto = new VoteReceiptRequestDto
        {
            CitizenId = citizenId.Value,
            ElectionId = electionId.Value,
            Items = new List<VoteItemDto>
            {
                new VoteItemDto
                {
                    PositionId = puestoId,
                    CandidateId = candidatoSeleccionadoId == 0 ? null : candidatoSeleccionadoId,
                    PartyId = partyId
                }
            }
        };

        var result = await _votacionService.CastVoteAsync(dto);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction("Opciones", new { numeroDocumento });
    }

    [HttpPost]
    public async Task<IActionResult> Finalizar(string numeroDocumento)
    {
        var citizenId = HttpContext.Session.GetInt32("CitizenId");
        var electionId = HttpContext.Session.GetInt32("ElectionId");
        var citizenEmail = HttpContext.Session.GetString("CitizenEmail");
        var citizenNombre = HttpContext.Session.GetString("CitizenNombre");

        if (citizenId == null || electionId == null)
            return RedirectToAction("Index", "Home");

        // Verificar que haya votado en todos los puestos activos
        var positionsResult = await _positionService.GetAllAsync();
        var activePositions = positionsResult.Data?.Where(p => p.IsActive).ToList() ?? new();
        var votedPositions = await _voteRepository.GetVotedPositionsAsync(citizenId.Value, electionId.Value);

        var puestosFaltantes = activePositions
            .Where(p => !votedPositions.Contains(p.Id))
            .Select(p => p.Nombre)
            .ToList();

        if (puestosFaltantes.Any())
        {
            TempData["PuestosFaltantes"] = string.Join(", ", puestosFaltantes);
            return RedirectToAction("Opciones", new { numeroDocumento });
        }

        // Obtener el voto del ciudadano para construir el comprobante
        var vote = await _voteRepository.GetVoteByCitizenAndElectionAsync(citizenId.Value, electionId.Value);

        // Enviar correo con comprobante detallado
        if (!string.IsNullOrEmpty(citizenEmail) && vote != null)
        {
            try
            {
                var electionResult = await _electionService.GetByIdAsync(electionId.Value);
                var electionName = electionResult.IsSuccess ? electionResult.Data!.Nombre : "Elección";

                var receiptItems = vote.VoteItems.Select(vi => new VoteReceiptItem
                {
                    PuestoNombre = vi.Position?.Nombre ?? "Puesto",
                    CandidatoNombre = vi.Candidate != null
                        ? $"{vi.Candidate.Nombre} {vi.Candidate.Apellido}"
                        : "Ninguno",
                    PartidoNombre = vi.Party?.Nombre ?? (vi.Candidate != null ? "" : "N/A")
                }).ToList();

                _ = _emailService.SendVoteReceiptAsync(
                    citizenEmail,
                    citizenNombre ?? "Ciudadano",
                    electionName,
                    receiptItems);

                _logger.LogInformation("Comprobante de voto enviado a {Email} para ciudadano {CitizenId}", citizenEmail, citizenId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar comprobante de voto al ciudadano {CitizenId}", citizenId);
            }
        }

        // Limpiar sesión del elector
        HttpContext.Session.Remove("CitizenId");
        HttpContext.Session.Remove("ElectionId");
        HttpContext.Session.Remove("NumeroDocumento");
        HttpContext.Session.Remove("CitizenEmail");
        HttpContext.Session.Remove("CitizenNombre");

        ViewBag.Message = "¡Su voto ha sido registrado exitosamente! Se ha enviado un comprobante a su correo electrónico. ¡Gracias por participar en el proceso electoral!";
        ViewBag.TipoMensaje = "success";
        return View("Mensaje");
    }

    [HttpGet]
    public IActionResult Mensaje() => View();
}
