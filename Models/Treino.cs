namespace Academia.Models
{ 
    public class Treino
    {
        public int TreinoID { get; set; }
        public string AlunoID { get; set; } // FK para Aluno
        public Aluno Aluno { get; set; }

        public string PersonalID { get; set; } // FK para Personal
        public Personal Personal { get; set; }

        public string NomeTreino { get; set; }
        public string Descricao { get; set; }

        public DateTime Data { get; set; }
        public DateTime Hora { get; set; }

        public ICollection<Exercicio> Exercicios { get; set; }
    }
}


