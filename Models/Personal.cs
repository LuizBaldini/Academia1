namespace Academia1.Models
{
    public class Personal : Usuario
    {           
        public string Especialidade { get; set; }
        public ICollection<Aluno> Alunos { get; set; }
        public ICollection<Treino> Treinos { get; set; }
    }
}
