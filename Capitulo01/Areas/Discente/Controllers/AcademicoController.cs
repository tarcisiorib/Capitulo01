using Capitulo01.Data;
using Capitulo01.Data.DAL.Discente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Modelo.Discente;
using System.IO;
using System.Threading.Tasks;

namespace Capitulo01.Areas.Discente.Controllers
{
    [Area("Discente")]
    [Authorize]
    public class AcademicoController : Controller
    {
        private readonly AcademicoDAO academicoDAO;
        private readonly IWebHostEnvironment env;

        public AcademicoController(IESContext context,
            IWebHostEnvironment env)
        {
            academicoDAO = new AcademicoDAO(context);
            this.env = env;
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
        public async Task<IActionResult> Edit(long? id, [Bind("AcademicoID,Nome,RegistroAcademico,Nascimento")]Academico academico, IFormFile foto, string removerFoto)
        {
            if (id != academico.AcademicoID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var stream = new MemoryStream();
                    if (removerFoto != null)
                        academico.Foto = null;
                    else
                    {
                        if (foto != null)
                        {
                            await foto.CopyToAsync(stream);
                            academico.Foto = stream.ToArray();
                            academico.MimeType = foto.ContentType;
                        }                        
                    }

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

        public async Task<FileContentResult> GetFoto(long id)
        {
            var academico = await academicoDAO.GetById(id);
            if (academico != null)
                return File(academico.Foto, academico.MimeType);

            return null;
        }

        public async Task<FileResult> Download(long id)
        {
            var academico = await academicoDAO.GetById(id);
            string arquivo = $"Foto {academico.AcademicoID.ToString().Trim()}.jpg";

            var fs = new FileStream(Path.Combine(env.WebRootPath, arquivo), FileMode.Create, FileAccess.Write);
            fs.Write(academico.Foto, 0, academico.Foto.Length);
            fs.Close();

            var provider = new PhysicalFileProvider(env.WebRootPath);
            var fi = provider.GetFileInfo(arquivo);
            var rs = fi.CreateReadStream();
            return File(rs, academico.MimeType, arquivo);
        }
    }
}
