using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Elections;
using eVote360.Domain.Enums;
using eVote360.Infrastructure.Persistence;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class EleccionesController : Controller
{
    private readonly IElectionService _electionService;
    private readonly AppDbContext _context;

    public EleccionesController(IElectionService electionService, AppDbContext context)
    {
        _electionService = electionService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _electionService.GetAllAsync();
        var elections = result.Data ?? Enumerable.Empty<eVote360.Domain.Entities.Election.Election>();

        var viewModels = elections.Select(e => new ElectionListVM
        {
            Id = e.Id,
            Nombre = e.Nombre,
            FechaRealizacion = e.FechaRealizacion,
            Estado = e.Estado == ElectionState.EnProceso ? "En Proceso" : "Finalizada",
            EsActiva = e.Estado == ElectionState.EnProceso,
            CreatedAt = e.CreatedAt
        }).ToList();

        return View(viewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var validation = await _electionService.ValidateCanCreateElectionAsync();
        if (!validation.IsSuccess)
        {
            TempData["ValidationErrors"] = string.Join("|", validation.Errors);
        }

        return View(new ElectionFormVM { FechaRealizacion = DateTime.Today.AddDays(7) });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ElectionFormVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _electionService.CreateElectionAsync(model.Nombre, model.FechaRealizacion);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Elección creada exitosamente";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmarFinalizar(int id)
    {
        var electionResult = await _electionService.GetByIdAsync(id);
        if (!electionResult.IsSuccess)
            return NotFound();

        ViewBag.ElectionId = id;
        ViewBag.ElectionNombre = electionResult.Data!.Nombre;
        return View("Finalizar");
    }

    [HttpPost]
    public async Task<IActionResult> Finalize(int id)
    {
        var result = await _electionService.FinalizeElectionAsync(id);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
        }
        else
        {
            TempData["Success"] = "Elección finalizada exitosamente";
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Resultados(int id)
    {
        var electionResult = await _electionService.GetByIdAsync(id);
        if (!electionResult.IsSuccess)
            return NotFound();

        var election = electionResult.Data!;

        // Cargar los votos con todos los detalles necesarios
        var votes = await _context.VoteItems
            .Include(vi => vi.Vote)
            .Include(vi => vi.Position)
            .Include(vi => vi.Candidate)
            .Include(vi => vi.Party)
            .Where(vi => vi.Vote.ElectionId == id)
            .ToListAsync();

        // Obtener puestos involucrados
        var positions = votes.Select(vi => vi.Position).DistinctBy(p => p?.Id)
            .Where(p => p != null).ToList();

        // Total de ciudadanos que votaron (votes únicos)
        var totalVotantes = await _context.Votes.CountAsync(v => v.ElectionId == id);

        var puestosVM = new List<ResultadoPuestoVM>();

        foreach (var position in positions.OrderBy(p => p?.Nombre))
        {
            if (position == null) continue;

            var voteItemsForPosition = votes.Where(vi => vi.PositionId == position.Id).ToList();
            var totalVotosPuesto = voteItemsForPosition.Count;

            // Agrupar por candidato
            var candidatoGroups = voteItemsForPosition
                .GroupBy(vi => new
                {
                    CandidatoId = vi.CandidateId,
                    CandidatoNombre = vi.Candidate != null
                        ? $"{vi.Candidate.Nombre} {vi.Candidate.Apellido}"
                        : "Ninguno",
                    PartidoNombre = vi.Party?.Nombre ?? "N/A",
                    PartidoSiglas = vi.Party?.Siglas ?? ""
                })
                .Select(g => new ResultadoCandidatoVM
                {
                    CandidatoNombre = g.Key.CandidatoNombre,
                    PartidoNombre = g.Key.PartidoNombre,
                    PartidoSiglas = g.Key.PartidoSiglas,
                    Votos = g.Count(),
                    Porcentaje = totalVotosPuesto > 0
                        ? Math.Round((double)g.Count() / totalVotosPuesto * 100, 2)
                        : 0
                })
                .OrderByDescending(c => c.Votos)
                .ToList();

            puestosVM.Add(new ResultadoPuestoVM
            {
                PuestoId = position.Id,
                PuestoNombre = position.Nombre,
                TotalVotosPuesto = totalVotosPuesto,
                Candidatos = candidatoGroups
            });
        }

        var vm = new ResultadosEleccionVM
        {
            EleccionId = id,
            EleccionNombre = election.Nombre,
            FechaRealizacion = election.FechaRealizacion,
            TotalVotos = totalVotantes,
            Puestos = puestosVM
        };

        return View(vm);
    }
}
