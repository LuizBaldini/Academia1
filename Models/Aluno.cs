using System.ComponentModel.DataAnnotations;
namespace Academia1.Models
{
    public class Aluno : Usuario
    {      

        [Required(ErrorMessage = "Selecione um Personal")]
        public string PersonalID { get; set; }

        // O objeto Personal pode ser nulo no início, não precisa ser Required
        public Personal? Personal { get; set; }

        // Inicializa como lista vazia para evitar erro de validação
        public ICollection<Treino> Treinos { get; set; } = new List<Treino>();
    }

}

