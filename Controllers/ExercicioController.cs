using Academia1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    public class ExercicioController : Controller
    {
        private readonly Context _context;
        public ExercicioController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var exercicios = _context.Exercicios.Include(e => e.Treinos);                
            return View(await exercicios.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var treino = _context.Exercicios.Include(e => e.Treinos).FirstOrDefault(e => e.ExercicioID == id);
            return View(treino);
        }

        public IActionResult Create()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercicio exercicio)
        {
            try
            {
                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Exercicio cadastrado com sucesso";
            }
            catch(Exception ex)
            {
                TempData["Mensagem"] = "Erro ao cadastrar o treino.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            return View(exercicio);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Exercicio exercicio)
        {
            try
            {
                _context.Update(exercicio);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Exercicio editado com sucesso";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = "Erro ao editar o treino.";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            return View(exercicio);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Exercicio exercicio)
        {
            try
            {
                _context.Exercicios.Remove(exercicio);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Exercicio excluido com sucesso";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = "Erro ao excluir o treino.";
            }
            return RedirectToAction("Index");
        }


    }
}
