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

        //busca os alunos do personal logado
        public async Task<IActionResult> Index()
        {
            var personalLogado = await _userManager.GetUserAsync(User);

            if (personalLogado is not Personal personal)
                return NotFound();

            var alunos = await _context.Users
                .OfType<Aluno>()
                .Where(a => a.PersonalID == personal.Id)
                .Include(a => a.Treinos)
                .ToListAsync();

            return View(alunos);
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

        //Editar seu oerfil
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



        public async Task<IActionResult> AlunosCadastrados()
        {
            var alunos = await _userManager.Users
                .OfType<Aluno>()
                .Include(a => a.Personal)
                .Include(a => a.Treinos)
                .ToListAsync();

            return View(alunos);
        }

        public async Task<IActionResult> DetailsAluno(string id)
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

        public async Task<IActionResult> CreateAluno()
        {
            var personais = await _context.Users
                .OfType<Personal>()
                .ToListAsync();
            ViewBag.Personais = new SelectList(personais, "Id", "UserName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAluno(Aluno aluno, string senha)
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

        public async Task<IActionResult> EditAluno(string id)
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
        public async Task<IActionResult> EditAluno(Aluno aluno, string novaSenha)
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

        public async Task<IActionResult> DeleteAluno(string id)
        {
            var aluno = await _userManager.Users
                .OfType<Aluno>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
                return NotFound();

            return View(aluno);
        }

        [HttpPost, ActionName("DeleteAluno")]
        public async Task<IActionResult> DeleteConfirmedAluno(string id)
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
