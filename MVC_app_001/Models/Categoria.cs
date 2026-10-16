using System.ComponentModel.DataAnnotations;
namespace MVC_app_001.Models
{
    public class Categoria
    {
        [Key] public int CategoriaId { get; set; }
        [Required] [StringLength(50)]public string Nombre { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
