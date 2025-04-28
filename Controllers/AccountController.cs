using Academia1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    //declaração dos gerenciadores de usuário, login e regras do identity
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    //configuração dos gerenciadores
    public AccountController(SignInManager<Usuario> signInManager,
                             UserManager<Usuario> userManager,
                             RoleManager<IdentityRole> roleManager) 
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }   

    [HttpGet]
    [HttpGet]
    public IActionResult Login()
    {
        // Verifica se o usuário já está autenticado
        if (_signInManager.IsSignedIn(User))
        {
            // Redireciona para a página correspondente com base no tipo de usuário (Personal ou Aluno)
            if (User.IsInRole("Personal"))
            {
                return RedirectToAction("MeusAlunos", "PersonalAluno");
            }
            else if (User.IsInRole("Aluno"))
            {
                return RedirectToAction("Index", "Aluno");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(RegisterViewModel model)
    {
        ModelState.Remove(nameof(RegisterViewModel.ConfirmPassword));

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Se o login for bem-sucedido, redireciona conforme o tipo de usuário
                if (user is Personal)
                {
                    return RedirectToAction("MeusAlunos", "PersonalAluno");
                }
                else if (user is Aluno)
                {
                    return RedirectToAction("Index", "Aluno");

                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }
        }

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

}