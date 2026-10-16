using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers.Api;

[ApiController]
[Route("api/eventos")]
[AllowAnonymous]
public class EventosApiController : ControllerBase
{
    private readonly AgendaDbContext _context;

    public EventosApiController(AgendaDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventoResponse>>> GetAll()
    {
        var eventos = await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.EstadoEvento)
            .ToListAsync();

        return Ok(eventos.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventoResponse>> GetById(int id)
    {
        var evento = await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.EstadoEvento)
            .SingleOrDefaultAsync(e => e.EventoId == id);

        return evento is null ? NotFound() : Ok(ToResponse(evento));
    }

    [HttpPost]
    public async Task<ActionResult<EventoResponse>> Create(EventoRequest request)
    {
        if (!await _context.Categorias.AnyAsync(c => c.CategoriaId == request.CategoriaId))
        {
            ModelState.AddModelError(nameof(request.CategoriaId), "La categoría no existe.");
        }

        if (request.EstadoEventoId.HasValue &&
            !await _context.Set<EstadoEvento>().AnyAsync(e => e.EstadoEventoId == request.EstadoEventoId))
        {
            ModelState.AddModelError(nameof(request.EstadoEventoId), "El estado no existe.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var evento = new Evento
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Lugar = request.Lugar,
            CategoriaId = request.CategoriaId,
            EstadoEventoId = request.EstadoEventoId
        };

        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = evento.EventoId }, await FindResponse(evento.EventoId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventoResponse>> Update(int id, EventoRequest request)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        if (!await _context.Categorias.AnyAsync(c => c.CategoriaId == request.CategoriaId))
        {
            ModelState.AddModelError(nameof(request.CategoriaId), "La categoría no existe.");
        }

        if (request.EstadoEventoId.HasValue &&
            !await _context.Set<EstadoEvento>().AnyAsync(e => e.EstadoEventoId == request.EstadoEventoId))
        {
            ModelState.AddModelError(nameof(request.EstadoEventoId), "El estado no existe.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        evento.Titulo = request.Titulo;
        evento.Descripcion = request.Descripcion;
        evento.FechaInicio = request.FechaInicio;
        evento.FechaFin = request.FechaFin;
        evento.Lugar = request.Lugar;
        evento.CategoriaId = request.CategoriaId;
        evento.EstadoEventoId = request.EstadoEventoId;
        await _context.SaveChangesAsync();

        return Ok(await FindResponse(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<EventoResponse?> FindResponse(int id)
    {
        var evento = await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.EstadoEvento)
            .Where(e => e.EventoId == id)
            .SingleOrDefaultAsync();

        return evento is null ? null : ToResponse(evento);
    }

    private static EventoResponse ToResponse(Evento e) =>
        new(e.EventoId, e.Titulo, e.Descripcion, e.FechaInicio, e.FechaFin, e.Lugar,
            e.CategoriaId, e.Categoria.Nombre, e.EstadoEventoId, e.EstadoEvento == null ? null : e.EstadoEvento.Nombre);
}

public sealed record EventoRequest(
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Lugar,
    int CategoriaId,
    int? EstadoEventoId);

public sealed record EventoResponse(
    int EventoId,
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Lugar,
    int CategoriaId,
    string Categoria,
    int? EstadoEventoId,
    string? Estado);
