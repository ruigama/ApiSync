using ApiSync.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSync.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connectionString = "";

            if(Configuration != null)
            {
                connectionString = Configuration.GetConnectionString("WebApiDatabase");
            }

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Funcionario>();
            modelBuilder.Entity<FuncionarioPesquisa>();
            modelBuilder.Entity<EscalaFuncionario>().HasNoKey();
            modelBuilder.Entity<Cargo>();
            modelBuilder.Entity<CentroCusto>();
            modelBuilder.Entity<Afastamento>();
            modelBuilder.Entity<PesquisaAfastamento>().HasNoKey();
            modelBuilder.Entity<TipoAfastamento>();
            modelBuilder.Entity<FuncionarioCargo>();
        }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<FuncionarioPesquisa> FuncionariosPesquisa { get; set; }
        public DbSet<EscalaFuncionario> FuncionarioEscala { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CentroCusto> CentroCustos { get; set; }
        public DbSet<Afastamento> Afastamentos { get; set; }
        public DbSet<PesquisaAfastamento> PesquisaAfastamentos { get; set; }
        public DbSet<TipoAfastamento> TipoAfastamentos { get; set; }
        public DbSet<FuncionarioCargo> FuncionarioCargos { get; set; }
    }
}
