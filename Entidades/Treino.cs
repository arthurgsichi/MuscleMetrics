namespace MuscleMetrics.Entidades
{
    public class Treino
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int Carga { get; set; }
        public int Repeticoes { get; set; }
        public int Series {  get; set; }
        public string Dia { get; set; }
        public string Exercicio { get; set; }
        public DateTime Data {  get; set; }

    }
}
