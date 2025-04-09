using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    //declaração dos gerenciadores de usuário, login e regras do identity
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    //configuração dos gerenciadores
    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // Action para exibir a página de registro
    [HttpGet]
    public IActionResult Register(string role)
    {
        ViewBag.Role = role;
        return View();
    }

    // Action para processar o registro (POST)
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, string role)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Adiciona o usuário à role correspondente (Professor ou Aluno)
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);

                // Faz login automático após o registro
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Exibe os erros caso o registro falhe
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ViewBag.Role = role;
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(RegisterViewModel model, string returnUrl = "/Home/Index")
    {
        // Remover a validação de ConfirmPassword no contexto do login
        ModelState.Remove(nameof(RegisterViewModel.ConfirmPassword));

        if (ModelState.IsValid)
        {
            // Verifique se o usuário existe
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine("Usuário não encontrado.");
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }

            Console.WriteLine($"Usuário encontrado: {user.UserName}");

            //busca o usuário no banco de dados
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                Console.WriteLine("Login bem-sucedido.");
                //se o login for bem sucedido
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (result.IsLockedOut)
            {
                Console.WriteLine("Usuário bloqueado.");
                ModelState.AddModelError(string.Empty, "Conta bloqueada. Tente novamente mais tarde.");
            }
            else if (result.RequiresTwoFactor)
            {
                Console.WriteLine("Autenticação de dois fatores necessária.");
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
            }
            else
            {
                Console.WriteLine("Tentativa de login inválida.");
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
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