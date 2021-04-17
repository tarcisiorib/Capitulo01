using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class DisciplinaDAO
    {
        private readonly IESContext _context;

        public DisciplinaDAO(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Disciplina> GetAll() => _context.Disciplinas;

        public async Task<Disciplina> GetById(long id) => await _context.Disciplinas.SingleOrDefaultAsync(d => d.DisciplinaID == id);

        public async Task<Disciplina> Upsert(Disciplina disciplina)
        {
            if (disciplina.DisciplinaID == null)
                _context.Disciplinas.Add(disciplina);
            else
                _context.Update(disciplina);

            await _context.SaveChangesAsync();

            return disciplina;
        }

        public async Task<Disciplina> Delete(long id)
        {
            var disciplina = await GetById(id);
            _context.Disciplinas.Remove(disciplina);
            await _context.SaveChangesAsync();
            return disciplina;
        }
    }
}
