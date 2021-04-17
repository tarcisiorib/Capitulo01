using Microsoft.EntityFrameworkCore;
using Modelo.Docente;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Docente
{
    public class ProfessorDAO
    {
        private readonly IESContext _context;

        public ProfessorDAO(IESContext context)
        {
            this._context = context;
        }

        public IQueryable<Professor> GetAll() => _context.Professores;

        public async Task<Professor> GetById(long id) => await _context.Professores.SingleOrDefaultAsync(p => p.ProfessorID == id);

        public async Task<Professor> Upsert(Professor professor)
        {
            if (professor.ProfessorID == null)
                _context.Professores.Add(professor);
            else
                _context.Update(professor);

            await _context.SaveChangesAsync();

            return professor;
        }

        public async Task<Professor> Delete(long id)
        {
            var professor = await GetById(id);
            _context.Professores.Remove(professor);
            await _context.SaveChangesAsync();
            return professor;
        }
    }
}
