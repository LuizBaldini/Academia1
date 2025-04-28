using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Academia1.Models
{
    public class Usuario : IdentityUser
    {
        [Display(Name = "Nome Completo")]
        public override string UserName { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime Data_Nascimento { get; set; }

        [StringLength(100, ErrorMessage = "O Instagram deve ter no máximo 100 caracteres.")]
        public string Instagram { get; set; }

        [Required(ErrorMessage = "Observações são obrigatórias.")]
        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres.")]
        public string Observacoes { get; set; }
    }
}
