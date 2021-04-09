using Capitulo01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capitulo01.Data
{
    public class IESDbInitializer
    {
        public static void Initialize(IESContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Departamentos.Any())
            {
                var departamentos = new Departamento[]
            {
                new Departamento { Nome = "Ciência da Computação" },
                new Departamento { Nome = "Ciência de Alimentos" }
            };

                foreach (Departamento d in departamentos)
                {
                    context.Departamentos.Add(d);
                }
            }

            if (!context.Instituicoes.Any())
            {
                var instituicoes = new Instituicao[]
                {
                    new Instituicao { Nome = "UniParaná", Endereco = "PR" },
                    new Instituicao { Nome = "UniPará", Endereco = "PA" }
                };

                foreach (Instituicao i in instituicoes)
                {
                    context.Instituicoes.Add(i);
                }
            }

            context.SaveChanges();
        }
    }
}
