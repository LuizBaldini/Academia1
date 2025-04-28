using Microsoft.EntityFrameworkCore;
using Academia1.Models;
using Microsoft.AspNetCore.Identity;

namespace Academia1.Models
{
    public class SeedData
    {
        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            context.Database.Migrate();

            string[] roles = { "Personal", "Aluno" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Criação de Personal padrão
            Personal personal = null;

            if (!userManager.Users.OfType<Personal>().Any())
            {
                personal = new Personal
                {
                    UserName = "Clinton Nogueira Silva",
                    Email = "clintonnogueirasilva@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "3599999-9999",
                    Data_Nascimento = new DateTime(2003, 7, 24),
                    Instagram = "@clinton.silvas",
                    Observacoes = "Personal padrão criado pelo Seed",
                    Especialidade = "Musculação"
                };

                var resultado = await userManager.CreateAsync(personal, "@Clinton123");
                if (!resultado.Succeeded)
                {
                    var erros = string.Join("\n", resultado.Errors.Select(e => $"Código: {e.Code} | Descrição: {e.Description}"));
                    throw new Exception("Erro ao criar o usuário:\n" + erros);
                }

                await userManager.AddToRoleAsync(personal, "Personal");
            }
            else
            {
                personal = await userManager.Users.OfType<Personal>().FirstOrDefaultAsync();
            }

            // Criação de Alunos
            if (!userManager.Users.OfType<Aluno>().Any())
            {
                var nomes = new List<string> { "João", "Maria", "José", "Lucas", "Ana", "Pedro", "Fernanda", "Carlos", "Juliana", "Rafael" };
                var sobrenomes = new List<string> { "Silva", "Santos", "Oliveira", "Souza", "Costa", "Pereira", "Almeida", "Martins", "Lima", "Rocha" };

                for (int i = 1; i <= 10; i++)
                {
                    var nome = $"{nomes[i % nomes.Count]} {sobrenomes[i % sobrenomes.Count]}";

                    var aluno = new Aluno
                    {
                        UserName = nome.ToLower().Replace(" ", ""),
                        Email = $"{nome.ToLower().Replace(" ", "")}@academia.com",
                        EmailConfirmed = true,
                        PhoneNumber = $"359999999{i:D2}",
                        Data_Nascimento = new DateTime(2000 + i % 5, 1 + (i % 12), 10),
                        Instagram = $"@aluno{i}",
                        Observacoes = $"Aluno gerado automaticamente - nº {i}",
                        PersonalID = personal?.Id
                    };

                    var result = await userManager.CreateAsync(aluno, $"Aluno@123{i}");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(aluno, "Aluno");
                    }
                    else
                    {
                        var erros = string.Join("\n", result.Errors.Select(e => $"Código: {e.Code} | Descrição: {e.Description}"));
                        throw new Exception($"Erro ao criar o aluno {i}:\n" + erros);
                    }
                }
            }

            // Criação de Exercícios
            if (!context.Exercicios.Any())
            {
                var exercicios = new List<Exercicio>
                {
                    new Exercicio { Nome = "Flexão de Braços", Categoria = "Peito", Descricao = "Exercício clássico para trabalhar o peito e tríceps." },
                    new Exercicio { Nome = "Agachamento", Categoria = "Pernas", Descricao = "Exercício fundamental para o fortalecimento das pernas e glúteos." },
                    new Exercicio { Nome = "Supino", Categoria = "Peito", Descricao = "Exercício de musculação para trabalhar o peito, ombro e tríceps." },
                    new Exercicio { Nome = "Puxada na Barra", Categoria = "Costas", Descricao = "Exercício que visa trabalhar os músculos das costas, principalmente o latíssimo do dorso." },
                    new Exercicio { Nome = "Rosca Direta", Categoria = "Braços", Descricao = "Exercício de musculação focado no bíceps." },
                    new Exercicio { Nome = "Leg Press", Categoria = "Pernas", Descricao = "Exercício de fortalecimento das pernas e glúteos, realizado em uma máquina." },
                    new Exercicio { Nome = "Abdominal", Categoria = "Core", Descricao = "Exercício que visa trabalhar a região do abdômen." },
                    new Exercicio { Nome = "Stiff", Categoria = "Pernas", Descricao = "Exercício que trabalha os músculos posteriores da coxa e glúteos." },
                    new Exercicio { Nome = "Cadeira Extensora", Categoria = "Pernas", Descricao = "Exercício que foca no fortalecimento do quadríceps." },
                    new Exercicio { Nome = "Tríceps Pulley", Categoria = "Braços", Descricao = "Exercício de musculação para fortalecer o tríceps." },
                    new Exercicio { Nome = "Desenvolvimento de Ombros", Categoria = "Ombros", Descricao = "Exercício para trabalhar os músculos dos ombros." },
                    new Exercicio { Nome = "Deadlift", Categoria = "Costas", Descricao = "Exercício que foca no fortalecimento das costas e posterior de coxa." },
                    new Exercicio { Nome = "Puxada Frente", Categoria = "Costas", Descricao = "Exercício para trabalhar o latíssimo do dorso com puxada de barra." },
                    new Exercicio { Nome = "Peck Deck", Categoria = "Peito", Descricao = "Exercício para isolar os músculos do peito." },
                    new Exercicio { Nome = "Panturrilha em Pé", Categoria = "Pernas", Descricao = "Exercício que trabalha a musculatura da panturrilha." },
                    new Exercicio { Nome = "Pull-up", Categoria = "Costas", Descricao = "Exercício de puxada para as costas, também trabalha o bíceps." },
                    new Exercicio { Nome = "Dumbbell Curl", Categoria = "Braços", Descricao = "Exercício de rosca direta utilizando halteres." },
                    new Exercicio { Nome = "Chest Fly", Categoria = "Peito", Descricao = "Exercício de peito com halteres, trabalhando o peitoral maior." },
                    new Exercicio { Nome = "Glúteo 4 Apoios", Categoria = "Glúteos", Descricao = "Exercício específico para glúteos e pernas." },
                    new Exercicio { Nome = "Mergulho", Categoria = "Braços", Descricao = "Exercício para trabalhar tríceps, peitoral e ombros." }
                };
                await context.Exercicios.AddRangeAsync(exercicios);
                await context.SaveChangesAsync();
            }

            // Criação de Treinos por categoria
            if (!context.Treinos.Any())
            {
                var alunos = await context.Users.OfType<Aluno>().ToListAsync();
                var random = new Random();

                foreach (var aluno in alunos)
                {
                    // peito
                    var treinoPeito = new Treino
                    {
                        NomeTreino = $"Treino de Peito - {aluno.UserName}",
                        Descricao = "Treino focado no peito.",
                        Data = DateTime.Today.AddDays(random.Next(1, 7)),
                        Hora = DateTime.Today.AddHours(random.Next(6, 10)).AddMinutes(random.Next(0, 60)),
                        AlunoID = aluno.Id,
                        PersonalID = personal.Id,
                        Exercicios = context.Exercicios.Where(e => e.Categoria == "Peito").OrderBy(x => Guid.NewGuid()).Take(4).ToList()
                    };
                    await context.Treinos.AddAsync(treinoPeito);

                    // pernas
                    var treinoPernas = new Treino
                    {
                        NomeTreino = $"Treino de Pernas - {aluno.UserName}",
                        Descricao = "Treino focado nas pernas.",
                        Data = DateTime.Today.AddDays(random.Next(1, 7)),
                        Hora = DateTime.Today.AddHours(random.Next(6, 10)).AddMinutes(random.Next(0, 60)),
                        AlunoID = aluno.Id,
                        PersonalID = personal.Id,
                        Exercicios = context.Exercicios.Where(e => e.Categoria == "Pernas").OrderBy(x => Guid.NewGuid()).Take(4).ToList()
                    };
                    await context.Treinos.AddAsync(treinoPernas);

                    // braços
                    var treinoBracos = new Treino
                    {
                        NomeTreino = $"Treino de Braços - {aluno.UserName}",
                        Descricao = "Treino focado nos braços.",
                        Data = DateTime.Today.AddDays(random.Next(1, 7)),
                        Hora = DateTime.Today.AddHours(random.Next(6, 10)).AddMinutes(random.Next(0, 60)),
                        AlunoID = aluno.Id,
                        PersonalID = personal.Id,
                        Exercicios = context.Exercicios.Where(e => e.Categoria == "Braços").OrderBy(x => Guid.NewGuid()).Take(4).ToList()
                    };
                    await context.Treinos.AddAsync(treinoBracos);

                    // costas
                    var treinoCostas = new Treino
                    {
                        NomeTreino = $"Treino de Costas - {aluno.UserName}",
                        Descricao = "Treino focado nas costas.",
                        Data = DateTime.Today.AddDays(random.Next(1, 7)),
                        Hora = DateTime.Today.AddHours(random.Next(6, 10)).AddMinutes(random.Next(0, 60)),
                        AlunoID = aluno.Id,
                        PersonalID = personal.Id,
                        Exercicios = context.Exercicios.Where(e => e.Categoria == "Costas").OrderBy(x => Guid.NewGuid()).Take(4).ToList()
                    };
                    await context.Treinos.AddAsync(treinoCostas);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
