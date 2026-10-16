namespace MVC_app_001.Models
{
    public class Auditoria
    {
        public int AuditoriaId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string Detalles { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}
