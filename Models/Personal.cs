using System.ComponentModel.DataAnnotations;
namespace Academia1.Models
{
    public class Personal : Usuario
    {
        [Required(ErrorMessage = "Espepecialidade é obrigatório.")]
        public string Especialidade { get; set; }
        public ICollection<Aluno> Alunos { get; set; }
        public ICollection<Treino> Treinos { get; set; }
    }
}
