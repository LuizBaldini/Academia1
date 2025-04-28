using Academia1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    [Authorize(Roles = "Personal")]
    public class TreinoController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<Usuario> _userManager;

        public TreinoController(Context context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var treinos = await _context.Treinos
                .Include(t => t.Aluno) // Carregar os dados do aluno associado ao treino
                .Select(t => new Treino
                {
                    TreinoID = t.TreinoID,
                    NomeTreino = t.NomeTreino,
                    Descricao = t.Descricao,
                    Aluno = t.Aluno // Associa o aluno ao treino
                })
                .ToListAsync();

            return View(treinos);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Aluno)
                .Include(t => t.Personal)
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            return View(treino);
        }

        public async Task<IActionResult> Create(string id = null)//
        {
            var personalLogado = await _userManager.GetUserAsync(User);

            var model = new TreinoViewModel
            {
                Treino = new Treino
                {
                    PersonalID = personalLogado.Id,
                    AlunoID = id // se o id vier, já seta aqui; se não, fica null
                },
                Alunos = await _context.Alunos
                    .Where(a => a.PersonalID == personalLogado.Id)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.UserName,
                        Selected = (id != null && a.Id == id) // marca como selecionado se o id vier
                    })
                    .ToListAsync(),
                Exercicios = await _context.Exercicios.ToListAsync()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(TreinoViewModel model, string returnUrl = null)
        {
            var personalLogado = await _userManager.GetUserAsync(User);
            model.Treino.PersonalID = personalLogado.Id;
            ModelState.Remove("Treino.PersonalID");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Mensagem"] = string.Join(" | ", errors);

                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index");
            }

            var treino = model.Treino;

            treino.Exercicios = await _context.Exercicios
                .Where(e => model.ExerciciosSelecionados.Contains(e.ExercicioID))
                .ToListAsync();

            _context.Treinos.Add(treino);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            
            var personalLogado = await _userManager.GetUserAsync(User);
            var treino = await _context.Treinos
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);
            

            // Verificar se o treino pertence ao personal logado
            if (treino.PersonalID != personalLogado.Id)
            {
                TempData["Mensagem"] = "Você não tem permissão para editar este treino.";
                return RedirectToAction("Index");
            }               

            var model = new TreinoViewModel
            {
                Treino = treino,
                Alunos = await _context.Alunos
                    .Where(a => a.PersonalID == personalLogado.Id) // Só alunos do personal
                    .Select(a => new SelectListItem { Value = a.Id, Text = a.UserName })
                    .ToListAsync(),
                Exercicios = await _context.Exercicios.ToListAsync(),
                ExerciciosSelecionados = treino.Exercicios.Select(e => e.ExercicioID).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TreinoViewModel model)
        {
            var personalLogado = await _userManager.GetUserAsync(User);
            ModelState.Remove("Treino.PersonalID");

            if (ModelState.IsValid)
            {
                try
                {
                    var treino = await _context.Treinos
                        .Include(t => t.Exercicios)
                        .FirstOrDefaultAsync(t => t.TreinoID == id);                    

                    treino.NomeTreino = model.Treino.NomeTreino;
                    treino.Descricao = model.Treino.Descricao;
                    treino.Data = model.Treino.Data;
                    treino.Hora = model.Treino.Hora;
                    treino.AlunoID = model.Treino.AlunoID;
                    treino.PersonalID = personalLogado.Id;

                    // Atualiza exercícios
                    treino.Exercicios.Clear();
                    var exerciciosSelecionados = await _context.Exercicios
                        .Where(e => model.ExerciciosSelecionados.Contains(e.ExercicioID))
                        .ToListAsync();
                    foreach (var ex in exerciciosSelecionados)
                    {
                        treino.Exercicios.Add(ex);
                    }

                    _context.Update(treino);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    if (!_context.Treinos.Any(e => e.TreinoID == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            model.Alunos = await _context.Alunos
                .Where(a => a.PersonalID == personalLogado.Id) // Só alunos do personal no caso de erro também
                .Select(a => new SelectListItem { Value = a.Id, Text = a.UserName })
                .ToListAsync();
            model.Exercicios = await _context.Exercicios.ToListAsync();

            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Aluno)
                .Include(t => t.Personal)
                .FirstOrDefaultAsync(m => m.TreinoID == id);

            if (treino == null) return NotFound();

            return View(treino);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treino = await _context.Treinos
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino != null)
            {
                treino.Exercicios.Clear(); // remove vínculo antes de deletar
                _context.Treinos.Remove(treino);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
