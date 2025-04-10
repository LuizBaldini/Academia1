using Academia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academia1.Controllers
{
    [Authorize(Roles = "Personal")]
    public class PersonalController : Controller
    {
        private readonly Context _context;

        public PersonalController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var lista = _context.Personais.ToList();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var personal = _context.Personais
                .Include(p => p.Alunos)
                .Include(p => p.Treinos)
                .FirstOrDefault(p => p.PersonalID == id);

            if (personal == null)
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
        public IActionResult Create(Personal personal)
        {
            if (ModelState.IsValid)
            {
                _context.Personais.Add(personal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(personal);
        }

        public IActionResult Edit(int id)
        {
            var personal = _context.Personais.Find(id);
            if (personal == null)
            {
                return NotFound();
            }
            return View(personal);
        }

       
        [HttpPost]
        public IActionResult Edit(int id, Personal personal)
        {
            if (id != personal.PersonalID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(personal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(personal);
        }

      
        public IActionResult Delete(int id)
        {
            var personal = _context.Personais.Find(id);
            if (personal == null)
            {
                return NotFound();
            }
            return View(personal);
        }

        }
    }
}
