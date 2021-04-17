using Capitulo01.Data;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System.Threading.Tasks;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    public class DisciplinaController : Controller
    {
        private readonly DisciplinaDAO disciplinaDAO;

        public DisciplinaController(IESContext context)
        {
            disciplinaDAO = new DisciplinaDAO(context);
        }

        public async Task<IActionResult> Index() => View(await disciplinaDAO.GetAll().ToListAsync());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome")] Disciplina disciplina)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await disciplinaDAO.Upsert(disciplina);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível salvar os dados.");
            }

            return View(disciplina);
        }

        public async Task<IActionResult> Edit(long? id) => await GetViewById(id);
        
        private async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var disciplina = await disciplinaDAO.GetById((long)id);

            if (disciplina == null)
                return NotFound();

            return View(disciplina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("DisciplinaID,Nome")] Disciplina disciplina)
        {
            if (id != disciplina.DisciplinaID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await disciplinaDAO.Upsert(disciplina);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DisciplinaExists(disciplina.DisciplinaID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(disciplina);
        }

        private async Task<bool> DisciplinaExists(long? id) => await disciplinaDAO.GetById((long)id) != null;

        public async Task<IActionResult> Details(long? id) => await GetViewById(id);

        public async Task<IActionResult> Delete(long? id) => await GetViewById(id);

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var disciplina = await disciplinaDAO.Delete(id);
            TempData["msg"] = $"{disciplina.Nome.ToUpper()} DELETED";
            return RedirectToAction(nameof(Index));
        }
    }
}
