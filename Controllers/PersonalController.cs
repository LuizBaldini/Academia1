using Academia1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    public class PersonalController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<Usuario> _userManager;

        public PersonalController(Context context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var personals = await _userManager.Users
                .OfType<Personal>()
                .Include(p => p.Alunos)
                .Include(p => p.Treinos)
                .ToListAsync();

            return View(personals);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var personal = await _userManager.Users
                .OfType<Personal>()
                .Include(p => p.Alunos)
                .Include(p => p.Treinos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personal == null)
                return NotFound();

            return View(personal);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Personal personal, string senha)
        {
            if (!ModelState.IsValid)
                return View(personal);

            try
            {
                var result = await _userManager.CreateAsync(personal, senha);

                if (result.Succeeded)
                {
                    TempData["Mensagem"] = "Personal cadastrado com sucesso";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao cadastrar o personal.";
            }

            return View(personal);
        }



        public async Task<IActionResult> Edit(string id)
        {
            var personal = await _context.Users
                .OfType<Personal>()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personal == null)
                return NotFound();

            return View(personal);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Personal personal, string novaSenha)
        {
            try
            {
                var personalExistente = await _userManager.Users
                    .OfType<Personal>()
                    .FirstOrDefaultAsync(p => p.Id == personal.Id);

                if (personalExistente == null)
                    return NotFound();

                // Atualiza manualmente os campos editáveis
                personalExistente.UserName = personal.UserName;
                personalExistente.Email = personal.Email;
                personalExistente.Data_Nascimento = personal.Data_Nascimento;
                personalExistente.Instagram = personal.Instagram;
                personalExistente.Observacoes = personal.Observacoes;
                personalExistente.Especialidade = personal.Especialidade;

                var result = await _userManager.UpdateAsync(personalExistente);

                if (!string.IsNullOrWhiteSpace(novaSenha))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(personalExistente);
                    var resultadoSenha = await _userManager.ResetPasswordAsync(personalExistente, token, novaSenha);

                    if (!resultadoSenha.Succeeded)
                    {
                        foreach (var error in resultadoSenha.Errors)
                            ModelState.AddModelError("", error.Description);

                        return View(personal); // Volta com erros de senha
                    }
                }

                if (result.Succeeded)
                {
                    TempData["Mensagem"] = "Personal editado com sucesso";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao editar o personal.";
            }

            return View(personal);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var personal = await _userManager.Users
                .OfType<Personal>()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personal == null)
                return NotFound();

            return View(personal);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var personal = await _userManager.Users
                .OfType<Personal>()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personal == null)
                return NotFound();

            try
            {
                var result = await _userManager.DeleteAsync(personal);

                if (result.Succeeded)
                {
                    TempData["Mensagem"] = "Personal excluído com sucesso";
                }
                else
                {
                    TempData["Mensagem"] = "Erro ao excluir o personal.";
                }
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Erro ao excluir o personal.";
            }

            return RedirectToAction("Index");
        }

    }
}
