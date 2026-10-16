using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers;

public class LoginController : Controller
{
    private readonly AgendaDbContext _context;

    public LoginController(AgendaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View("~/Views/Login/Register.cshtml");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authenticate(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Home/Index.cshtml", model);
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Correo == model.Correo);

        if (usuario is null || usuario.Contraseña != model.Contraseña)
        {
            ModelState.AddModelError(string.Empty, "El correo o la contraseña son incorrectos.");
            return View("~/Views/Home/Index.cshtml", model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Correo)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = model.Recordarme
            });

        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Eventos")!;
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(LoginViewModel model, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(nameof(nombre), "El nombre es obligatorio.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Login/Register.cshtml", model);
        }

        var correoExiste = await _context.Usuarios
            .AnyAsync(u => u.Correo == model.Correo);

        if (correoExiste)
        {
            ModelState.AddModelError(nameof(model.Correo), "Ese correo ya está registrado.");
            return View("~/Views/Login/Register.cshtml", model);
        }

        _context.Usuarios.Add(new Usuario
        {
            Nombre = nombre.Trim(),
            Correo = model.Correo.Trim(),
            Contraseña = model.Contraseña,
            FechaRegistro = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["RegisterSuccess"] = "Usuario creado correctamente. Ya puedes iniciar sesión.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }
}
