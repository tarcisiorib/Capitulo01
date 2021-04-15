﻿using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Discente;
using Capitulo01.Models.Infra;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Capitulo01.Data
{
    public class IESContext : IdentityDbContext<Usuario>
    {
        public IESContext(DbContextOptions<IESContext> options) : base(options)
        {

        }

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Academico> Academicos{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Departamento>().ToTable("Departamento");
            modelBuilder.Entity<Instituicao>().ToTable("Instituicao");
            modelBuilder.Entity<Curso>().ToTable("Curso");
            modelBuilder.Entity<Disciplina>().ToTable("Disciplina");
            modelBuilder.Entity<CursoDisciplina>().ToTable("CursoDisciplina");
            modelBuilder.Entity<Academico>().ToTable("Academico");

            modelBuilder.Entity<CursoDisciplina>()
                .HasKey(cd => new { cd.CursoID, cd.DisciplinaID });

            modelBuilder.Entity<CursoDisciplina>()
                .HasOne(c => c.Curso)
                .WithMany(cd => cd.CursosDisciplinas)
                .HasForeignKey(c => c.CursoID);

            modelBuilder.Entity<CursoDisciplina>()
                .HasOne(d => d.Disciplina)
                .WithMany(cd => cd.CursosDisciplinas)
                .HasForeignKey(d => d.DisciplinaID);
        }
    }
}
