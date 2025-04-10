using Academia1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Academia.Models
{
    public class Context : IdentityDbContext<Usuario>
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Personal> Personais { get; set; }
        public DbSet<Treino> Treinos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configura o Discriminator
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("Tipo")
                .HasValue<Usuario>("Usuario")
                .HasValue<Aluno>("Aluno")
                .HasValue<Personal>("Personal");

            // Relacionamento Aluno -> Personal (muitos para um)
            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Personal)
                .WithMany(p => p.Alunos)
                .HasForeignKey(a => a.PersonalID)
                .OnDelete(DeleteBehavior.Restrict); // evita ciclos

            // Relacionamento Treino -> Aluno (muitos para um)
            modelBuilder.Entity<Treino>()
                .HasOne(t => t.Aluno)
                .WithMany(a => a.Treinos)
                .HasForeignKey(t => t.AlunoID)
                .OnDelete(DeleteBehavior.Restrict); // evita cascata múltipla

            // Relacionamento Treino -> Personal (muitos para um)
            modelBuilder.Entity<Treino>()
                .HasOne(t => t.Personal)
                .WithMany(p => p.Treinos)
                .HasForeignKey(t => t.PersonalID)
                .OnDelete(DeleteBehavior.Restrict); // evita cascata múltipla

            // Relacionamento muitos-para-muitos entre Treino e Exercicio
            modelBuilder.Entity<Treino>()
                .HasMany(t => t.Exercicios)
                .WithMany(e => e.Treinos)
                .UsingEntity(j => j.ToTable("TreinoExercicio"));
        }
    }
}
