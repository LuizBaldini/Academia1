using Academia1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
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
            var alunos = await _userManager.Users
                .OfType<Aluno>()
                .Include(a => a.Personal)
                .Include(a => a.Treinos)
                .ToListAsync();

            return View(alunos);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .Include(a => a.Personal)
                .Include(a => a.Treinos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
                return NotFound();

            return View(aluno);
        }

        public async Task<IActionResult> Create()
        {            
            var personais = await _context.Users
                .OfType<Personal>()
                .ToListAsync();
            ViewBag.Personais = new SelectList(personais, "Id", "UserName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Aluno aluno, string senha)
        {
            if (!ModelState.IsValid)
            {
                var pessoais = await _context.Users
                    .OfType<Personal>()
                    .ToListAsync();
                ViewBag.Personais = new SelectList(pessoais, "Id", "UserName");
                return View(aluno);
            }

            try
            {
                var result = await _userManager.CreateAsync(aluno, senha);

                if (result.Succeeded)
                {
                    // Adiciona o usuário à role "Aluno"
                    await _userManager.AddToRoleAsync(aluno, "Aluno");

                    TempData["Mensagem"] = "Aluno cadastrado com sucesso";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao cadastrar o aluno.";
            }

            var pessoaisOnPost = await _userManager.GetUsersInRoleAsync("Personal");
            ViewBag.Personais = new SelectList(pessoaisOnPost, "Id", "UserName");
            return View(aluno);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var aluno = await _userManager.FindByIdAsync(id);                

            if (aluno == null)
                return NotFound();

            var personais = await _context.Users
                 .OfType<Personal>()
                 .ToListAsync();
            ViewBag.Personais = new SelectList(personais, "Id", "UserName");

            return View(aluno);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Aluno aluno, string novaSenha)
        {
           
            try
            {
                var alunoExistente = await _userManager.Users
                    .OfType<Aluno>()
                    .FirstOrDefaultAsync(a => a.Id == aluno.Id);

                if (alunoExistente == null)
                    return NotFound();

                alunoExistente.UserName = aluno.UserName;
                alunoExistente.Email = aluno.Email;
                alunoExistente.Data_Nascimento = aluno.Data_Nascimento;
                alunoExistente.Instagram = aluno.Instagram;
                alunoExistente.Observacoes = aluno.Observacoes;
                alunoExistente.PersonalID = aluno.PersonalID;

                var result = await _userManager.UpdateAsync(alunoExistente);

                if (!string.IsNullOrWhiteSpace(novaSenha))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(alunoExistente);
                    var resultadoSenha = await _userManager.ResetPasswordAsync(alunoExistente, token, novaSenha);

                    if (!resultadoSenha.Succeeded)
                    {
                        foreach (var error in resultadoSenha.Errors)
                            ModelState.AddModelError("", error.Description);

                        return View(aluno); // Volta para a view com erro
                    }
                }

                if (result.Succeeded)
                {
                    TempData["Mensagem"] = "Aluno editado com sucesso";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao editar o aluno.";
            }

            return View(aluno);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
                return NotFound();

            return View(aluno);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
                return NotFound();

            try
            {
                var result = await _userManager.DeleteAsync(aluno);

                TempData["Mensagem"] = result.Succeeded
                    ? "Aluno excluído com sucesso"
                    : "Erro ao excluir o aluno.";
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao excluir o aluno.";
            }

            return RedirectToAction("Index");
        }
    }
}
