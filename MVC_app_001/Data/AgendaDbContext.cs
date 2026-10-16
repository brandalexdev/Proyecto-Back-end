using Microsoft.EntityFrameworkCore;
using MVC_app_001.Models;
namespace MVC_app_001.Data
{
    public class AgendaDbContext: DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<MVC_app_001.Models.Recordatorio> Recordatorio { get; set; } = default!;
        public DbSet<MVC_app_001.Models.Auditoria> Auditoria { get; set; } = default!;
    }
}

