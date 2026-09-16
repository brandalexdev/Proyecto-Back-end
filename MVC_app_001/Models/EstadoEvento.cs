namespace MVC_app_001.Models
{
    public class EstadoEvento
    {
    public int EstadoEventoId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public ICollection<Evento> Eventos { get; set; }

        = new List<Evento>();
    }
}

