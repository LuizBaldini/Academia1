using Academia.Models.Academia.Models;

namespace Academia.Models
{
    public class Personal
    {
        public int PersonalID { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }

        public ICollection<Aluno> Alunos { get; set; }
        public ICollection<Treino> Treinos { get; set; }
    }

}
