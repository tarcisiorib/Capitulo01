using Capitulo01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Modelo.Cadastros;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Authorization;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class DepartamentoController : Controller
    {
        private DepartamentoDAO departamentoDAO;
        private InstituicaoDAO InstituicaoDAO;

        public DepartamentoController(IESContext context)
        {
            departamentoDAO = new DepartamentoDAO(context);
            InstituicaoDAO = new InstituicaoDAO(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await departamentoDAO.GetAll().ToListAsync());
        }

        public IActionResult Create()
        {
            var instituicoes = InstituicaoDAO.GetAll().ToList();
            instituicoes.Insert(0, new Instituicao() { InstituicaoID = 0, Nome = "Selecione uma instituição" });
            ViewBag.Instituicoes = instituicoes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome","InstituicaoID")] Departamento departamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await departamentoDAO.Upsert(departamento);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível inserir os dados.");
            }

            return View(departamento);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            ViewResult visaoDepartamento = (ViewResult)await GetViewById(id);
            var departamento = (Departamento)visaoDepartamento.Model;
            ViewBag.Instituicoes = new SelectList(InstituicaoDAO.GetAll(), "InstituicaoID", "Nome", departamento.InstituicaoID);
            return visaoDepartamento;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("DepartamentoID,Nome","InstituicaoID")] Departamento departamento)
        {
            if (id != departamento.DepartamentoID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await departamentoDAO.Upsert(departamento);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartamentoExists(departamento.DepartamentoID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Instituicoes = new SelectList(InstituicaoDAO.GetAll(), "InstituicaoID", "Nome", departamento.InstituicaoID);
            return View(departamento);
        }

        private async Task<bool> DepartamentoExists(long? id)
        {
            return await departamentoDAO.GetById((long)id) != null;
        }

        public async Task<IActionResult> Details(long? id)
        {
            return await GetViewById(id);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            return await GetViewById(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var departamento = await departamentoDAO.Delete((long)id);
            TempData["msg"] = $"Departamento {departamento.Nome.ToUpper()} foi excluído";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var departamento = await departamentoDAO.GetById((long)id);

            if (departamento == null)
                return NotFound();

            return View(departamento);
        }
    }
}
