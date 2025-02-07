using Microsoft.Identity.Client;

namespace MuscleMetrics.Entidades
{
    public class NivelAtividadeFisica
    {
        public string AtividadesForaAcademia { get; set; }
        public int TempoTreino { get; set; }
        public int FrequenciaTreino { get; set; }
        public int IntensidadeTreino { get; set; }
        public int ?TempoAeróbico { get; set; }

       
    }
}
