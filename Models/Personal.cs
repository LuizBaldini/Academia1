using Academia1.Models;
using System.ComponentModel.DataAnnotations;

namespace Academia.Models
{
    public class Personal: Usuario
    {
        [StringLength(100, ErrorMessage = "As observações devem ter no máximo 100 caracteres.")]        
        public string Especialidade { get; set; }
        public ICollection<Aluno> Alunos { get; set; }
        public ICollection<Treino> Treinos { get; set; }
    }

}
