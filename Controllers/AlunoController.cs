using Academia1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class AlunoController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<Usuario> _userManager;

        public AlunoController(Context context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userLogado = await _userManager.GetUserAsync(User);

            if (userLogado is not Aluno aluno)
                return NotFound();

            var treinos = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Exercicios)
                .Where(t => t.AlunoID == aluno.Id)
                .ToListAsync();

            return View(treinos);
        }
        public async Task<IActionResult> Perfil()
        {
            var userId = _userManager.GetUserId(User); 

            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .Include(a => a.Personal)
                .Include(a => a.Treinos)
                .FirstOrDefaultAsync(a => a.Id == userId);

            if (aluno == null)
                return NotFound();

            return View(aluno);
        }

        [HttpGet]
        public async Task<IActionResult> Editar()
        {
            var userId = _userManager.GetUserId(User);  

            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .FirstOrDefaultAsync(a => a.Id == userId);

            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Aluno alunoEditado)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);  

                var aluno = await _userManager.Users
                    .OfType<Aluno>()
                    .FirstOrDefaultAsync(a => a.Id == userId);

                if (aluno == null)
                {
                    return NotFound();
                }

                aluno.Email = alunoEditado.Email;
                aluno.PhoneNumber = alunoEditado.PhoneNumber;
                aluno.Instagram = alunoEditado.Instagram;
                aluno.Observacoes = alunoEditado.Observacoes;
                aluno.Data_Nascimento = alunoEditado.Data_Nascimento;

                var result = await _userManager.UpdateAsync(aluno);

                if (result.Succeeded)
                {
                    TempData["Mensagem"] = "Dados atualizados com sucesso!";
                    return RedirectToAction("Perfil");  
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(alunoEditado);
        }


        // Ação para listar os treinos do aluno logado
        public async Task<IActionResult> MeusTreinos()
        {
            var userId = _userManager.GetUserId(User);

            var treinos = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Exercicios)
                .Where(t => t.AlunoID == userId)
                .ToListAsync();

            return View(treinos);
        }

        // Ação para ver detalhes de um treino
        public async Task<IActionResult> DetalhesTreino(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var treino = await _context.Treinos
                .Include(t => t.Personal)
                .Include(t => t.Exercicios)
                .FirstOrDefaultAsync(t => t.TreinoID == id && t.AlunoID == userId);

            if (treino == null) return NotFound();

            return View(treino);
        }
    }
}
