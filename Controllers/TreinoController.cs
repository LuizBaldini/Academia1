using Academia1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    [Authorize(Roles = "Personal")]
    public class TreinoController : Controller
    {
        private readonly Context _context;

        public TreinoController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var treinos = await _context.Treinos
                .Select(t => new Treino
                {
                    TreinoID = t.TreinoID,
                    NomeTreino = t.NomeTreino,
                    Descricao = t.Descricao
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


        public async Task<IActionResult> Create()
        {
            var model = new TreinoViewModel
            {
                Treino = new Treino(), 
                Alunos = await _context.Alunos
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.UserName })
                    .ToListAsync(),
                Personais = await _context.Personais
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.UserName })
                    .ToListAsync(),
                Exercicios = await _context.Exercicios.ToListAsync()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreinoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Mensagem"] = string.Join(" | ", errors); // ou logue no console
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var treino = model.Treino;

                treino.Exercicios = await _context.Exercicios
                    .Where(e => model.ExerciciosSelecionados.Contains(e.ExercicioID))
                    .ToListAsync();

                _context.Treinos.Add(treino);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Recarrega os dados para o form
            model.Alunos = await _context.Alunos
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.UserName })
                .ToListAsync();

            model.Personais = await _context.Personais
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.UserName })
                .ToListAsync();

            model.Exercicios = await _context.Exercicios.ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var treino = await _context.Treinos
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id);

            if (treino == null) return NotFound();

            var model = new TreinoViewModel
            {
                Treino = treino,
                Alunos = await _context.Alunos.Select(a => new SelectListItem { Value = a.Id, Text = a.UserName }).ToListAsync(),
                Personais = await _context.Personais.Select(p => new SelectListItem { Value = p.Id, Text = p.UserName }).ToListAsync(),
                Exercicios = await _context.Exercicios.ToListAsync(),
                ExerciciosSelecionados = treino.Exercicios.Select(e => e.ExercicioID).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TreinoViewModel model)
        {
            if (id != model.Treino.TreinoID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var treino = await _context.Treinos
                        .Include(t => t.Exercicios)
                        .FirstOrDefaultAsync(t => t.TreinoID == id);

                    if (treino == null) return NotFound();

                    treino.NomeTreino = model.Treino.NomeTreino;
                    treino.Descricao = model.Treino.Descricao;
                    treino.Data = model.Treino.Data;
                    treino.Hora = model.Treino.Hora;
                    treino.AlunoID = model.Treino.AlunoID;
                    treino.PersonalID = model.Treino.PersonalID;

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
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Treinos.Any(e => e.TreinoID == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            // Recarregar dropdowns se der erro
            model.Alunos = await _context.Alunos.Select(a => new SelectListItem { Value = a.Id, Text = a.UserName }).ToListAsync();
            model.Personais = await _context.Personais.Select(p => new SelectListItem { Value = p.Id, Text = p.UserName }).ToListAsync();
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
