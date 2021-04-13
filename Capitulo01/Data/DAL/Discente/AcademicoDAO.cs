using Modelo.Discente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Discente
{
    public class AcademicoDAO
    {
        private IESContext _context;

        public AcademicoDAO(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Academico> GetAll()
        {
            return _context.Academicos;
        }

        public async Task<Academico> GetById(long id)
        {
            return await _context.Academicos.FindAsync(id);
        }

        public async Task<Academico> Upsert(Academico academico)
        {
            if (academico.AcademicoID == null)
                _context.Academicos.Add(academico);
            else
                _context.Update(academico);

            await _context.SaveChangesAsync();
            return academico;
        }

        public async Task<Academico> Delete(long id)
        {
            var academico = await GetById(id);
            _context.Academicos.RemoveRange(academico);
            await _context.SaveChangesAsync();
            return academico;
        }
    }
}
