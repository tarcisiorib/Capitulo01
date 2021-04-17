using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Docente;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class CursoDAO
    {
        private readonly IESContext _context;

        public CursoDAO(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Curso> GetAll()
        {
            return _context.Cursos.Include(d => d.Departamento);
        }

        public async Task<Curso> GetById(long id)
        {
            var curso = await _context.Cursos.SingleOrDefaultAsync(c => c.CursoID == id);
            _context.Departamentos.Where(d => curso.DepartamentoID == d.DepartamentoID).Load();
            return curso;
        }

        public async Task<Curso> Upsert(Curso curso)
        {
            if (curso.CursoID == null)
                _context.Cursos.Add(curso);
            else
                _context.Update(curso);

            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task<Curso> Delete(long id)
        {
            var curso = await GetById(id);
            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();
            return curso;
        }


        public void RegistrarProfessor(long cursoID, long professorID)
        {
            var curso = _context.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursosProfessores).First();
            var professor = _context.Professores.Find(professorID);
            curso.CursosProfessores.Add(new CursoProfessor() { Curso = curso, Professor = professor });
            _context.SaveChanges();
        }

        public IQueryable<Professor> ObterProfessoresForaDoCurso(long cursoID)
        {
            var curso = _context.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursosProfessores).First();
            var professoresDoCurso = curso.CursosProfessores.Select(cp => cp.ProfessorID).ToArray();
            var professoresForaDoCurso = _context.Professores.Where(p => !professoresDoCurso.Contains(p.ProfessorID));
            return professoresForaDoCurso;
        }
    }
}
