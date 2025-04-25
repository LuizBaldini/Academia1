using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Academia1.Models
{
    public class Treino
    {
        public int TreinoID { get; set; }

        [Required(ErrorMessage = "O nome do treino é obrigatório.")]
        public string NomeTreino { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A hora é obrigatória.")]
        public DateTime Hora { get; set; }

        [Required(ErrorMessage = "É necessário selecionar um aluno.")]
        public string AlunoID { get; set; }

        [ValidateNever]
        public Aluno Aluno { get; set; }

        [Required(ErrorMessage = "É necessário selecionar um personal.")]
        public string PersonalID { get; set; }

        [ValidateNever]
        public Personal Personal { get; set; }

        public ICollection<Exercicio> Exercicios { get; set; } = new List<Exercicio>();
    }

}


