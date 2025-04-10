using Academia1.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Academia.Models
{
    public class Aluno : Usuario
    {
        public string PersonalID { get; set; }
        public Personal Personal { get; set; }
        public ICollection<Treino> Treinos { get; set; }
    }
}
