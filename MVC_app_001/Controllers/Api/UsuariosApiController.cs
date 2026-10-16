using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers.Api;

[ApiController]
[Route("api/usuarios")]
[AllowAnonymous]
public class UsuariosApiController : ControllerBase
{
    private readonly AgendaDbContext _context;

    public UsuariosApiController(AgendaDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponse>>> GetAll() =>
        Ok(await Query().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> GetById(int id)
    {
        var user = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UsuarioId == id);

        return user is null ? NotFound() : Ok(ToResponse(user));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Create(UsuarioRequest request)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo))
        {
            ModelState.AddModelError(nameof(request.Correo), "El correo ya está registrado.");
            return ValidationProblem(ModelState);
        }

        var user = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Contraseña = request.Contraseña,
            FechaRegistro = request.FechaRegistro ?? DateTime.UtcNow
        };
        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.UsuarioId }, await FindResponse(user.UsuarioId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> Update(int id, UsuarioRequest request)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo && u.UsuarioId != id))
        {
            ModelState.AddModelError(nameof(request.Correo), "El correo ya está registrado.");
            return ValidationProblem(ModelState);
        }

        user.Nombre = request.Nombre;
        user.Correo = request.Correo;
        user.Contraseña = request.Contraseña;
        user.FechaRegistro = request.FechaRegistro ?? user.FechaRegistro;
        await _context.SaveChangesAsync();
        return Ok(await FindResponse(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<UsuarioResponse> Query() =>
        _context.Usuarios.AsNoTracking().Select(u =>
            new UsuarioResponse(u.UsuarioId, u.Nombre, u.Correo, u.FechaRegistro));

    private async Task<UsuarioResponse?> FindResponse(int id)
    {
        var user = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UsuarioId == id);

        return user is null ? null : ToResponse(user);
    }

    private static UsuarioResponse ToResponse(Usuario user) =>
        new(user.UsuarioId, user.Nombre, user.Correo, user.FechaRegistro);
}

public sealed record UsuarioRequest(
    string Nombre,
    string Correo,
    string Contraseña,
    DateTime? FechaRegistro);

public sealed record UsuarioResponse(
    int UsuarioId,
    string Nombre,
    string Correo,
    DateTime FechaRegistro);
