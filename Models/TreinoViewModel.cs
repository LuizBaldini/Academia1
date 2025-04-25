using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Academia1.Models;
public class TreinoViewModel
{
    public Treino Treino { get; set; }

    [BindNever]
    [ValidateNever]
    public IEnumerable<SelectListItem> Alunos { get; set; }

    [BindNever]
    [ValidateNever]
    public IEnumerable<SelectListItem> Personais { get; set; }

    [BindNever]
    [ValidateNever]
    public List<Exercicio> Exercicios { get; set; }


    public List<int> ExerciciosSelecionados { get; set; } = new();
}
