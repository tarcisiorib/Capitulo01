using Capitulo01.Areas.Docente.Models;
using Capitulo01.Data;
using Capitulo01.Data.DAL.Cadastros;
using Capitulo01.Data.DAL.Docente;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Docente;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Areas.Docente.Controllers
{
    [Area("Docente")]
    public class ProfessorController : Controller
    {        
        private readonly ProfessorDAO professorDAO;
        private readonly InstituicaoDAO instituicaoDAO;
        private readonly DepartamentoDAO departamentoDAO;
        private readonly CursoDAO cursoDAO;

        public ProfessorController(IESContext context)
        {
            professorDAO = new ProfessorDAO(context);
            instituicaoDAO = new InstituicaoDAO(context);
            departamentoDAO = new DepartamentoDAO(context);
            cursoDAO = new CursoDAO(context);
        }
        public async Task<IActionResult> Index() => View(await professorDAO.GetAll().ToListAsync());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome")] Professor professor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await professorDAO.Upsert(professor);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível salvar os dados");
            }

            return View(professor);
        }

        public async Task<IActionResult> Edit(long? id) => await GetViewById(id);
        
        private async Task<IActionResult> GetViewById(long? id)
        {
            if (id == null)
                return BadRequest();

            var professor = await professorDAO.GetById((long)id);

            if (professor == null)
                return NotFound();

            return View(professor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("ProfessorID,Nome")] Professor professor)
        {
            if (id != professor.ProfessorID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await professorDAO.Upsert(professor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProfessorExists(professor.ProfessorID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(professor);
        }

        private async Task<bool> ProfessorExists(long? id) => await professorDAO.GetById((long)id) != null;

        public async Task<IActionResult> Details(long? id) => await GetViewById(id);

        public async Task<IActionResult> Delete(long? id) => await GetViewById(id);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var professor = await professorDAO.GetById(id);
            TempData["msg"] = $"Professor {professor.Nome.ToUpper()} excluído!";
            return RedirectToAction(nameof(Index));
        }

        public void PrepararViewBags(List<Instituicao> instituicoes, List<Departamento> departamentos, List<Curso> cursos, List<Professor> professores)
        {
            instituicoes.Insert(0, new Instituicao() { InstituicaoID = 0, Nome = "Selecione a instituição" });
            ViewBag.Instituicoes = instituicoes;
            departamentos.Insert(0, new Departamento() { DepartamentoID = 0, Nome = "Selecione o departamento" });
            ViewBag.Departamentos = departamentos;
            cursos.Insert(0, new Curso() { CursoID = 0, Nome = "Selecione o curso" });
            ViewBag.Cursos = cursos;
            professores.Insert(0, new Professor() { ProfessorID = 0, Nome = "Selecione o professor" });
            ViewBag.Professores = professores;
        }

        [HttpGet]
        public IActionResult AdicionarProfessor()
        {
            PrepararViewBags(instituicaoDAO.GetAll().ToList(), new List<Departamento>().ToList(), new List<Curso>().ToList(), new List<Professor>());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarProfessor([Bind("InstituicaoID,DepartamentoID,CursoID,ProfessorID")] AdicionarProfessorViewModel model)
        {
            if (model.InstituicaoID == 0 || model.DepartamentoID == 0 || model.CursoID == 0 || model.ProfessorID == 0)
                ModelState.AddModelError(string.Empty, "É preciso selecionar todos os dados");
            else
            {
                cursoDAO.RegistrarProfessor((long)model.CursoID, (long)model.ProfessorID);
                RegistrarProfessorNaSessao((long)model.CursoID, (long)model.ProfessorID);
                PrepararViewBags(instituicaoDAO.GetAll().ToList(),
                    departamentoDAO.GetAll().Where(m => m.InstituicaoID == (long)model.InstituicaoID).ToList(),
                    cursoDAO.GetAll().Where(c => c.DepartamentoID == (long)model.DepartamentoID).ToList(),
                    cursoDAO.ObterProfessoresForaDoCurso((long)model.CursoID).ToList());
            }
            return View(model);
        }

        public JsonResult ObterDepartamentosPorInstituicao(long actionID)
        {
            var departamentos = departamentoDAO.GetAll().Where(d => d.InstituicaoID == actionID).ToList();
            return Json(new SelectList(departamentos, "DepartamentoID", "Nome"));
        }

        public JsonResult ObterCursosPorDepartamento(long actionID)
        {
            var cursos = cursoDAO.GetAll().Where(c => c.DepartamentoID == actionID).ToList();
            return Json(new SelectList(cursos, "CursoID", "Nome"));
        }

        public JsonResult ObterProfessoresForaDoCurso(long actionID)
        {
            var professores = cursoDAO.ObterProfessoresForaDoCurso(actionID).ToList();
            return Json(new SelectList(professores, "ProfessorID", "Nome"));
        }

        public void RegistrarProfessorNaSessao(long cursoID, long professorID)
        {
            var cursoProfessor = new CursoProfessor() { ProfessorID = professorID, CursoID = cursoID };
            List<CursoProfessor> cursosProfessor = new List<CursoProfessor>();
            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");
            if (cursosProfessoresSession != null)
                cursosProfessor = JsonConvert.DeserializeObject<List<CursoProfessor>>(cursosProfessoresSession);

            cursosProfessor.Add(cursoProfessor);
            HttpContext.Session.SetString("cursosProfessores", JsonConvert.SerializeObject(cursosProfessor));
        }

        public IActionResult VerificarUltimosRegistros()
        {
            var cursosProfessor = new List<CursoProfessor>();
            string cursosProfessoresSession = HttpContext.Session.GetString("cursosProfessores");
            if (cursosProfessoresSession != null)
                cursosProfessor = JsonConvert.DeserializeObject<List<CursoProfessor>>(cursosProfessoresSession);

            return View(cursosProfessor);
        }
    }
}
