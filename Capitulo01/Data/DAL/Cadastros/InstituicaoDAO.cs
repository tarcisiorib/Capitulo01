using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data.DAL.Cadastros
{
    public class InstituicaoDAO
    {
        private readonly IESContext _context;

        public InstituicaoDAO(IESContext context)
        {
            _context = context;
        }

        public IQueryable<Instituicao> GetAll()
        {
            return _context.Instituicoes;
        }

        public async Task<Instituicao> GetById(long id)
        {
            return await _context.Instituicoes.Include(d => d.Departamentos).SingleOrDefaultAsync(i => i.InstituicaoID == id);
        }

        public async Task<Instituicao> Upsert(Instituicao instituicao)
        {
            if (instituicao.InstituicaoID == null)
                _context.Instituicoes.Add(instituicao);
            else
                _context.Update(instituicao);

            await _context.SaveChangesAsync();
            return instituicao;               
        }

        public async Task<Instituicao> Delete(long id)
        {
            Instituicao instituicao = await GetById(id);
            _context.Instituicoes.Remove(instituicao);
            await _context.SaveChangesAsync();
            return instituicao;
        }
    }
}
