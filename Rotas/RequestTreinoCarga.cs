namespace MuscleMetrics.Rotas
{
    public class RequestTreinoCarga
    {
        public string Nome {  get; set; }
        public string Exercicio { get; set; }
        public double Carga { get; set; }
        public int Repeticoes { get; set; }
        public DateTime Data { get; set; }
    }
}
