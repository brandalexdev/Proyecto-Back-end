using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers.Api;

[ApiController]
[Route("api/recordatorios")]
[AllowAnonymous]
public class RecordatoriosApiController : ControllerBase
{
    private readonly AgendaDbContext _context;

    public RecordatoriosApiController(AgendaDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecordatorioResponse>>> GetAll() =>
        Ok(await Query().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecordatorioResponse>> GetById(int id)
    {
        var item = await _context.Recordatorio
            .AsNoTracking()
            .Include(r => r.Evento)
            .SingleOrDefaultAsync(r => r.recordatorioId == id);

        return item is null ? NotFound() : Ok(ToResponse(item));
    }

    [HttpPost]
    public async Task<ActionResult<RecordatorioResponse>> Create(RecordatorioRequest request)
    {
        if (!await _context.Eventos.AnyAsync(e => e.EventoId == request.EventoId))
        {
            ModelState.AddModelError(nameof(request.EventoId), "El evento no existe.");
            return ValidationProblem(ModelState);
        }

        var item = new Recordatorio
        {
            FechaRecordatorio = request.FechaRecordatorio,
            mensaje = request.Mensaje,
            Enviado = request.Enviado,
            EventoId = request.EventoId
        };
        _context.Recordatorio.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.recordatorioId }, await FindResponse(item.recordatorioId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RecordatorioResponse>> Update(int id, RecordatorioRequest request)
    {
        var item = await _context.Recordatorio.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        if (!await _context.Eventos.AnyAsync(e => e.EventoId == request.EventoId))
        {
            ModelState.AddModelError(nameof(request.EventoId), "El evento no existe.");
            return ValidationProblem(ModelState);
        }

        item.FechaRecordatorio = request.FechaRecordatorio;
        item.mensaje = request.Mensaje;
        item.Enviado = request.Enviado;
        item.EventoId = request.EventoId;
        await _context.SaveChangesAsync();
        return Ok(await FindResponse(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Recordatorio.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        _context.Recordatorio.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<RecordatorioResponse> Query() =>
        _context.Recordatorio
            .AsNoTracking()
            .Include(r => r.Evento)
            .Select(r => new RecordatorioResponse(
                r.recordatorioId, r.FechaRecordatorio, r.mensaje, r.Enviado, r.EventoId,
                r.Evento.Titulo));

    private async Task<RecordatorioResponse?> FindResponse(int id)
    {
        var item = await _context.Recordatorio
            .AsNoTracking()
            .Include(r => r.Evento)
            .SingleOrDefaultAsync(r => r.recordatorioId == id);

        return item is null ? null : ToResponse(item);
    }

    private static RecordatorioResponse ToResponse(Recordatorio item) =>
        new(item.recordatorioId, item.FechaRecordatorio, item.mensaje, item.Enviado,
            item.EventoId, item.Evento.Titulo);
}

public sealed record RecordatorioRequest(
    DateTime FechaRecordatorio,
    string Mensaje,
    bool Enviado,
    int EventoId);

public sealed record RecordatorioResponse(
    int RecordatorioId,
    DateTime FechaRecordatorio,
    string Mensaje,
    bool Enviado,
    int EventoId,
    string Evento);
