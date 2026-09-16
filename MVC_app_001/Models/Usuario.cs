using System.ComponentModel.DataAnnotations;

namespace MVC_app_001.Models
{
    public class Usuario
    {
        [Key] public int UsuarioId { get; set; }
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] [EmailAddress] public string Correo { get; set; } = string.Empty;
        [Required] public string Contraseña { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Evento> Eventos { get; set; }
            = new List<Evento>();

       // public virtual ICollection<Auditoria> Auditorias { get; set; }
           // = new List<Auditoria>();
    }
}