namespace MVC_app_001.Models
{
    public class Recordatorio
    {
        public int recordatorioId { get; set; }

        public DateTime FechaRecordatorio { get; set; }

        public string mensaje { get; set; } = string.Empty;

        public bool Enviado { get; set; }

        public int EventoId { get; set; }

        public Evento Evento { get; set; }

    }

}

