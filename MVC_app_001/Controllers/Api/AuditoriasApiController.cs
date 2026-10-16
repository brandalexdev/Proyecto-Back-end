using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers.Api;

[ApiController]
[Route("api/auditorias")]
[AllowAnonymous]
public class AuditoriasApiController : ControllerBase
{
    private readonly AgendaDbContext _context;

    public AuditoriasApiController(AgendaDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Auditoria>>> GetAll() =>
        Ok(await _context.Auditoria.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Auditoria>> GetById(int id)
    {
        var item = await _context.Auditoria.AsNoTracking().SingleOrDefaultAsync(a => a.AuditoriaId == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Auditoria>> Create(Auditoria request)
    {
        request.AuditoriaId = 0;
        _context.Auditoria.Add(request);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = request.AuditoriaId }, request);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Auditoria>> Update(int id, Auditoria request)
    {
        var item = await _context.Auditoria.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        item.Usuario = request.Usuario;
        item.Accion = request.Accion;
        item.FechaHora = request.FechaHora;
        item.Detalles = request.Detalles;
        item.UsuarioId = request.UsuarioId;
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Auditoria.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        _context.Auditoria.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
