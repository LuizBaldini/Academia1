using System.ComponentModel.DataAnnotations;

namespace Academia1.Models
{
    public class Exercicio
    {
        public int ExercicioID { get; set; }

        [Required(ErrorMessage = "O nome do exercício é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do exercício não pode ter mais de 100 caracteres.")]
        [Display(Name = "Nome do Exercício")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public ICollection<Treino> Treinos { get; set; }
    }

}


