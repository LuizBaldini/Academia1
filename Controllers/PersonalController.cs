using Academia1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Academia1.Controllers
{
    [Authorize(Roles = "Personal")]
    public class PersonalController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<Usuario> _userManager;

        public PersonalController(Context context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Perfil()
        {
            var usuarioLogado = await _userManager.GetUserAsync(User);

            if (usuarioLogado is not Personal personal)
            {
                return NotFound(); 
            }

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
                    // Adiciona o usuário à role "Aluno"
                    await _userManager.AddToRoleAsync(personal, "Personal");
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

        //Editar seu perfil
        public async Task<IActionResult> Edit()
        {
            var personal = await _userManager.GetUserAsync(User) as Personal;

            if (personal == null)
                return NotFound();

            return View(personal);
        }

        //post editar seu perfil
        [HttpPost]
        public async Task<IActionResult> Edit(Personal personalAtualizado, string novaSenha)
        {
            var personal = await _userManager.GetUserAsync(User) as Personal;

            if (personal == null)
                return NotFound();

            // Atualiza os dados permitidos
            personal.UserName = personalAtualizado.UserName;
            personal.Email = personalAtualizado.Email;
            personal.Data_Nascimento = personalAtualizado.Data_Nascimento;
            personal.Instagram = personalAtualizado.Instagram;
            personal.Observacoes = personalAtualizado.Observacoes;
            personal.Especialidade = personalAtualizado.Especialidade;

            var result = await _userManager.UpdateAsync(personal);

            if (!string.IsNullOrWhiteSpace(novaSenha))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(personal);
                var resultadoSenha = await _userManager.ResetPasswordAsync(personal, token, novaSenha);

                if (!resultadoSenha.Succeeded)
                {
                    foreach (var error in resultadoSenha.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(personalAtualizado);
                }
            }

            if (result.Succeeded)
            {
                TempData["Mensagem"] = "Dados atualizados com sucesso.";
                return RedirectToAction("Perfil");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(personalAtualizado);
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
            // Busca o personal com os alunos associados
            var personal = await _userManager.Users
                .OfType<Personal>()
                .Include(p => p.Alunos)  // Inclui os alunos associados
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personal == null)
                return NotFound();

            try
            {
                // Verifica se o personal tem alunos associados
                if (personal.Alunos.Any())
                {
                    TempData["Mensagem"] = "Não é possível excluir o personal enquanto houver alunos associados.";
                    return RedirectToAction("Index");
                }

                // Se não houver alunos associados, pode prosseguir com a exclusão
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
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao excluir o personal. Detalhes: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
