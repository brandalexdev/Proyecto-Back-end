using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MVC_app_001.Models
{
    public class Evento
    {
        [Key] public int EventoId { get; set; }
        [Required] [StringLength(100)] public string Titulo { get; set; } = string.Empty;
        [StringLength(2000)] public string Descripcion { get; set; } = string.Empty;
        [Required]public DateTime FechaInicio { get; set; }
        [Required]public DateTime FechaFin { get; set; }
        [Required]public string Lugar { get; set; } = string.Empty;
        [ForeignKey("Usuario")] public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        [ForeignKey("Categoria")] public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey("EstadoEvento")] public int EstadoEventoId { get; set; }
        public EstadoEvento EstadoEvento { get; set; } = null!;


    }
}

