using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace MuscleMetrics.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public double Peso { get; set; }
        public double Altura { get; set; }
        public double BF { get; set; }
        public string Sexo { get; set; }
        public double TMB { get; set; }
        public double FatorAtividade { get; set; }
        public double GastoCalórico { get; set; }
        public int DiasDeTreinoNaSemana { get; set; }
        public Usuario()
        {

        }

        public Usuario(string nome, int idade, double peso, double altura, double BF, string sexo, double TMB)
        {
            Nome = nome;
            Idade = idade;
            Peso = peso;
            Altura = altura;
            this.BF = BF;
            Sexo = sexo;
            this.TMB = TMB;
        }
    }
}

