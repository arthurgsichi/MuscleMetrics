namespace MuscleMetrics.Entidades
{
    public class Dieta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public double CaloriaDieta { get; set; }
        public double Proteina { get; set; }
        public double Carboidrato { get; set; }
        public double Gordura { get; set; }
        public DateTime DataRegistro { get; set; }
        public double Peso{ get; set; }
        public double Altura{ get; set; }
        public double BF{ get; set; }
        public Dieta()
        {

        }

        public Dieta(Usuario user, int usuarioId, double caloriaDieta, DateTime dataRegistro)
        {

            UsuarioId = user.Id;
            CaloriaDieta = caloriaDieta;
            /*Proteina = proteina;
            Carboidrato = carboidrato;
            Gordura = gordura;*/
            DataRegistro = dataRegistro;
        }
    }
}
