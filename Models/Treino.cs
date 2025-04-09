namespace Academia.Models
{
  namespace Academia.Models
{
    public class Treino
    {
        public int TreinoID { get; set; }

        public int AlunoID { get; set; } // FK para Aluno
        public Aluno Aluno { get; set; }

        public int PersonalID { get; set; } // FK para Personal
        public Personal Personal { get; set; }

        public DateTime Data { get; set; }
        public DateTime Hora { get; set; }

        public ICollection<Exercicio> Exercicios { get; set; } = new List<Exercicio>();
    }
}

}
