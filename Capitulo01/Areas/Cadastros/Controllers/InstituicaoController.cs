using Capitulo01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Modelo.Cadastros;
using Capitulo01.Data.DAL.Cadastros;
using Microsoft.AspNetCore.Authorization;

namespace Capitulo01.Areas.Cadastros.Controllers
{
    [Area("Cadastros")]
    [Authorize]
    public class InstituicaoController : Controller
    {        
        private readonly InstituicaoDAO instituicaoDAO;

        public InstituicaoController(IESContext context)
        {            
            instituicaoDAO = new InstituicaoDAO(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await instituicaoDAO.GetAll().ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome","Endereco")]Instituicao instituicao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await instituicaoDAO.Upsert(instituicao);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível inserir os dados.");
            }

            return View(instituicao);
        }

        private async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var instituicao = await instituicaoDAO.GetById((long)id);

            if (instituicao == null)
                return NotFound();

            return View(instituicao);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            return await GetViewById(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("InstituicaoID,Nome,Endereco")] Instituicao instituicao)
        {
            if (id != instituicao.InstituicaoID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await instituicaoDAO.Upsert(instituicao);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await InstituicaoExists(instituicao.InstituicaoID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(instituicao);
        }

        private async Task<bool> InstituicaoExists(long? id)
        {
            return await instituicaoDAO.GetById((long) id) != null;
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
            var instituicao = await instituicaoDAO.Delete((long) id);
            TempData["msg"] = $"Instituição {instituicao.Nome.ToUpper()} foi excluída!";
            return RedirectToAction(nameof(Index));
        }
    }
}
