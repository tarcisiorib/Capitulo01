using Capitulo01.Data;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System.Threading.Tasks;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class CursoController : Controller
    {
        private readonly CursoDAO cursoDAO;
        private readonly DepartamentoDAO departamentoDAO;

        public CursoController(IESContext context)
        {
            cursoDAO = new CursoDAO(context);
            departamentoDAO = new DepartamentoDAO(context);
        }

        public async Task<IActionResult> Index() => View(await cursoDAO.GetAll().ToListAsync());

        public async Task<IActionResult> Create()
        {
            var departamentos = await departamentoDAO.GetAll().ToListAsync();
            departamentos.Insert(0, new Departamento() { DepartamentoID = 0, Nome = "Selecione um departamento" });
            ViewBag.Departamentos = departamentos;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DepartamentoID")] Curso curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await cursoDAO.Upsert(curso);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível inserir os dados.");
            }

            return View(curso);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            var view = (ViewResult)await GetViewById(id);
            var model = (Curso)view.Model;
            ViewBag.Departamentos = new SelectList(departamentoDAO.GetAll(), "DepartamentoID", "Nome", model.DepartamentoID);
            return view;
        }

        private async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var curso = await cursoDAO.GetById((long)id);

            if (curso == null)
                return NotFound();

            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("CursoID,Nome,DepartamentoID")] Curso curso)
        {
            if (id != curso.CursoID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await cursoDAO.Upsert(curso);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CursoExists(curso.DepartamentoID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departamentos = new SelectList(departamentoDAO.GetAll(), "DepartamentoID", "Nome",  curso.DepartamentoID);
            return View(curso);
        }

        private async Task<bool> CursoExists(long? id) => await cursoDAO.GetById((long)id) != null;

        public async Task<IActionResult> Details(long? id) => await GetViewById(id);

        public async Task<IActionResult> Delete(long? id) => await GetViewById(id);

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var curso = await cursoDAO.Delete((long)id);
            TempData["msg"] = $"Curso {curso.Nome.ToUpper()} excluído!";
            return RedirectToAction(nameof(Index));
        }
    }
}
