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
