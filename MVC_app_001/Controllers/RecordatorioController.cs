using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;
using MVC_app_001.Models;

namespace MVC_app_001.Controllers
{
    public class RecordatorioController : Controller
    {
        private readonly AgendaDbContext _context;

        public RecordatorioController(AgendaDbContext context)
        {
            _context = context;
        }

        // GET: Recordatorio
        public async Task<IActionResult> Index()
        {
            var agendaDbContext = _context.Recordatorio.Include(r => r.Evento);
            return View(await agendaDbContext.ToListAsync());
        }

        // GET: Recordatorio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordatorio = await _context.Recordatorio
                .Include(r => r.Evento)
                .FirstOrDefaultAsync(m => m.recordatorioId == id);
            if (recordatorio == null)
            {
                return NotFound();
            }

            return View(recordatorio);
        }

        // GET: Recordatorio/Create
        public IActionResult Create()
        {
            ViewData["EventoId"] = new SelectList(_context.Eventos, "EventoId", "Lugar");
            return View();
        }

        // POST: Recordatorio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("recordatorioId,FechaRecordatorio,mensaje,Enviado,EventoId")] Recordatorio recordatorio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recordatorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "EventoId", "Lugar", recordatorio.EventoId);
            return View(recordatorio);
        }

        // GET: Recordatorio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordatorio = await _context.Recordatorio.FindAsync(id);
            if (recordatorio == null)
            {
                return NotFound();
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "EventoId", "Lugar", recordatorio.EventoId);
            return View(recordatorio);
        }

        // POST: Recordatorio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("recordatorioId,FechaRecordatorio,mensaje,Enviado,EventoId")] Recordatorio recordatorio)
        {
            if (id != recordatorio.recordatorioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recordatorio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordatorioExists(recordatorio.recordatorioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "EventoId", "Lugar", recordatorio.EventoId);
            return View(recordatorio);
        }

        // GET: Recordatorio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordatorio = await _context.Recordatorio
                .Include(r => r.Evento)
                .FirstOrDefaultAsync(m => m.recordatorioId == id);
            if (recordatorio == null)
            {
                return NotFound();
            }

            return View(recordatorio);
        }

        // POST: Recordatorio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recordatorio = await _context.Recordatorio.FindAsync(id);
            if (recordatorio != null)
            {
                _context.Recordatorio.Remove(recordatorio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordatorioExists(int id)
        {
            return _context.Recordatorio.Any(e => e.recordatorioId == id);
        }
    }
}
