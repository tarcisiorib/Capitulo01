using Capitulo01.Data;
using Capitulo01.Data.DAL.Discente;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelo.Discente;
using System.Threading.Tasks;

namespace Capitulo01.Controllers
{
    public class AcademicoController : Controller
    {
        private readonly AcademicoDAO academicoDAO;

        public AcademicoController(IESContext context)
        {
            academicoDAO = new AcademicoDAO(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await academicoDAO.GetAll().ToListAsync());
        }

        public async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var academico = await academicoDAO.GetById((long)id);

            if (academico == null)
                return NotFound();

            return View(academico);
        }

        public async Task<IActionResult> Details(long? id)
        {
            return await GetViewById(id);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            return await GetViewById(id);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            return await GetViewById(id);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,RegistroAcademico,Nascimento")]Academico academico)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await academicoDAO.Upsert(academico);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível inserir os dados.");
            }

            return View(academico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("AcademicoID,Nome,RegistroAcademico,Nascimento")]Academico academico)
        {
            if (id != academico.AcademicoID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await academicoDAO.Upsert(academico);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AcademicoExists(academico.AcademicoID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(academico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var academico = await academicoDAO.Delete((long)id);
            TempData["msg"] = $"Acadêmico {academico.Nome.ToUpper()} foi excluído.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AcademicoExists(long? id)
        {
            return await academicoDAO.GetById((long)id) != null;
        }
    }
}
