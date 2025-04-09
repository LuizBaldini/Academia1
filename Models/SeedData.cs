using Academia.Models;
using Academia.Models.Academia.Models;
using Microsoft.EntityFrameworkCore;

namespace Academia.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            Context context = app.ApplicationServices.GetRequiredService<Context>();
            context.Database.Migrate();

            if (context.Alunos.Any() || context.Personais.Any() || context.Exercicios.Any() || context.Treinos.Any())
            {
                return; // Já possui dados, não precisa popular novamente
            }

            // Exercícios
            var ex1 = new Exercicio
            {
                Nome = "Agachamento Livre",
                Categoria = "Perna",
                Descricao = "Agachamento com barra livre para trabalhar quadríceps e glúteos"
            };

            var ex2 = new Exercicio
            {
                Nome = "Supino Reto",
                Categoria = "Peito",
                Descricao = "Supino reto com barra para trabalhar peitoral maior"
            };

            var ex3 = new Exercicio
            {
                Nome = "Remada Curvada",
                Categoria = "Costas",
                Descricao = "Exercício para dorsais utilizando barra ou halteres"
            };

            var ex4 = new Exercicio
            {
                Nome = "Tricips Corda",
                Categoria = "Tricips",
                Descricao = "Exercício para tricips utilizando a corda no crossover"
            };

            // Personais
            var p1 = new Personal
            {
                Nome = "Carlos Silva",
                Especialidade = "Hipertrofia"
            };

            var p2 = new Personal
            {
                Nome = "Juliana Costa",
                Especialidade = "Emagrecimento"
            };

            // Alunos
            var a1 = new Aluno
            {
                Nome = "Lucas Pereira",
                Data_Nascimento = new DateTime(1998, 5, 21),
                E_mail = "lucas.pereira@gmail.com",
                Instagram = "@lucaspfit",
                Telefone = "(35) 99999-1234",
                Observacoes = "Tem histórico de lesão no joelho",
                Personal = p1
            };

            var a2 = new Aluno
            {
                Nome = "Ana Souza",
                Data_Nascimento = new DateTime(2000, 11, 10),
                E_mail = "ana.souza@hotmail.com",
                Instagram = "@anasfit",
                Telefone = "(35) 98888-5678",
                Observacoes = "Objetivo: ganho de massa magra",
                Personal = p2
            };

            // Adiciona Exercícios, Personais e Alunos
            context.AddRange(ex1, ex2, ex3, ex4);
            context.AddRange(p1, p2);
            context.AddRange(a1, a2);
            context.SaveChanges();

            // Treinos com IDs reais
            var t1 = new Treino
            {
                PersonalID = p1.PersonalID   ,
                AlunoID = a1.AlunoID,
                Data = new DateTime(2025, 4, 8),
                Hora = new DateTime(2025, 4, 8, 8, 0, 0),
                Exercicios = new List<Exercicio> { ex1, ex2, ex4 }
            };

            var t2 = new Treino
            {
                PersonalID = p2.PersonalID,
                AlunoID = a2.AlunoID,
                Data = new DateTime(2025, 4, 9),
                Hora = new DateTime(2025, 4, 9, 17, 30, 0),
                Exercicios = new List<Exercicio> { ex1, ex2, ex3 }
            };

            context.AddRange(t1, t2);
            context.SaveChanges();
        }
    }
}
