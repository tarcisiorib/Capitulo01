using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class DepartamentoDAO
    {
        private IESContext _context;

        public DepartamentoDAO(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Departamento> GetAll()
        {
            return _context.Departamentos.Include(d => d.Instituicao);
        }

        public async Task<Departamento> GetById(long id)
        {
            var departamento = await _context.Departamentos.SingleOrDefaultAsync(d => d.DepartamentoID == id);
            _context.Instituicoes.Where(i => departamento.InstituicaoID == i.InstituicaoID).Load();
            return departamento;
        }

        public async Task<Departamento> Upsert(Departamento departamento)
        {
            if (departamento.DepartamentoID == null)
                _context.Departamentos.Add(departamento);
            else
                _context.Update(departamento);

            await _context.SaveChangesAsync();
            return departamento;
        }

        public async Task<Departamento> Delete(long id)
        {
            var departamento = await GetById((long)id);
            _context.Departamentos.Remove(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }
    }
}
