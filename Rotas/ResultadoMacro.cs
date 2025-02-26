using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuscleMetrics.Data;
using MuscleMetrics.Entidades;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace MuscleMetrics.Rotas
{
    public static class ResultadoMacro
    {
        public static void AddRotasResultadoMacro(this WebApplication app)
        {
            // Endpoint para calcular a TMB e armazenar no banco
            app.MapPost("/CalcularTaxaMetabolicaBasal", async ([FromServices] AppDbContext context, RequestTBM request, CancellationToken ct) =>
            {
                var usuarioExistente = await context.Usuario
                    .FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower());

                if (usuarioExistente == null)
                {
                    usuarioExistente = new Usuario(request.Nome, request.Idade, request.Peso, request.Altura, request.BF, request.Sexo, 0);
                    await context.Usuario.AddAsync(usuarioExistente, ct);
                    await context.SaveChangesAsync(ct);
                }
                else
                {
                    usuarioExistente.Altura = request.Altura;
                    usuarioExistente.Peso = request.Peso;
                    usuarioExistente.BF = request.BF;
                    usuarioExistente.Idade = request.Idade;
                    usuarioExistente.Sexo = request.Sexo;
                }

                CalcularTMB(usuarioExistente);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new
                {
                    Usuario = usuarioExistente.Nome,
                    usuarioExistente.TMB
                });


            });

            // Endpoint para calcular gasto calórico total com base no nível de atividade
            app.MapPost("/CalcularGastoCalorico", async ([FromServices] AppDbContext context, [FromBody] RequestGastoCalorico request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower());
                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                usuario.FatorAtividade = ObterFatorAtividade(request.NivelAtividadeFisica_1_a_5);
                usuario.GastoCalórico = usuario.TMB * usuario.FatorAtividade;
                await context.SaveChangesAsync(ct);

                return Results.Ok(new
                {
                    Usuario = usuario.Nome,
                    Gasto_Calórico = usuario.GastoCalórico
                });
            });

            app.MapPost("/EscolherObjetivoUsuario", async ([FromServices] AppDbContext context, [FromBody] RequestObjetivo request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower().Equals(request.Nome.ToLower()));
                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }
                var dietaExistente = await context.Dieta.Where(d => d.UsuarioId == usuario.Id).ToListAsync();
               
                    var novaDieta = new Dieta
                    {
                        UsuarioId = usuario.Id,
                        CaloriaDieta = 0,
                        DataRegistro = DateTime.Now,
                        Altura = usuario.Altura,
                        Peso = usuario.Peso,
                        BF = usuario.BF,
                    };


                    switch (request.Objetivo.ToLower())
                    {
                        case "cutting":
                            CalculoCutting(novaDieta, usuario);
                            break;
                        case "bulking":
                            CalculoBulking(novaDieta, usuario);
                            break;
                        case "manutenção":
                            CalculoManutencao(novaDieta, usuario);
                            break;
                        default:
                            return Results.NotFound("Objetivo de dieta inválido.");
                    }
                    context.Dieta.Add(novaDieta);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok(new
                    {
                        Usuario = usuario.Nome,
                        DietaResultado = CalculoCutting(novaDieta, usuario)
                    });                
            });

            app.MapPost("/EvolucaoUsuario", async ([FromServices] AppDbContext context, [FromBody] RequestEvolucao request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.NomeUsuario.ToLower());
                if (usuario == null)
                {
                    Results.NotFound("Usuário não registrado");
                }
                var evolucoes = await context.Dieta
                    .Where(d => d.UsuarioId == usuario.Id)
                    .OrderBy(d => d.DataRegistro)
                    .Select(d => new
                    {
                        DataRegistro = d.DataRegistro.ToString("dd/MM/yyyy"),
                        Peso = d.Peso,
                        Altura = d.Altura,
                        BF = d.BF
                    })
                    .ToListAsync();

                if (evolucoes == null || !evolucoes.Any())
                    return Results.NotFound("Nenhum registro de evolução encontrado para o usuário.");

                return Results.Ok(evolucoes);
            });

            app.MapPost("/EscolhaTreino", async ([FromServices] AppDbContext context, [FromBody] RequestTreino request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower());
                if (usuario == null)
                {
                    Results.NotFound("Usuário ainda não foi cadastrado");
                }
                var listaTreinos = new List<Treino>();

                switch (usuario.Sexo)
                {
                    case "Masculino":
                        listaTreinos = new List<Treino>
                {
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Pull-up (barra fixa)", Series = 4, Repeticoes = 6 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Remada Curvada", Series = 4, Repeticoes = 8 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Desenvolvimento com Barra", Series = 4, Repeticoes = 8 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia B", Exercicio = "Agachamento Livre", Series = 4, Repeticoes = 8 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia B", Exercicio = "Leg Press", Series = 4, Repeticoes = 10 }
                };
                        break;

                    case "Feminino":
                        listaTreinos = new List<Treino>
                        {
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Agachamento Livre", Series = 4, Repeticoes = 10 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Hip Thrust", Series = 4, Repeticoes = 8 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia A", Exercicio = "Leg Press 45°", Series = 4, Repeticoes = 12 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia B", Exercicio = "Desenvolvimento com Halteres", Series = 4, Repeticoes = 10 },
                    new Treino { UsuarioId = usuario.Id, Dia = "Dia B", Exercicio = "Elevação Lateral", Series = 4, Repeticoes = 12 }
                };
                        break;

                    default:
                        return Results.NotFound("Sexo não especificado.");
                }

                await context.Treinos.AddRangeAsync(listaTreinos);
                await context.SaveChangesAsync();

                return Results.Ok("Treino salvo com sucesso.");
            });

            app.MapPost("/VisualizarTreinoAtual", async ([FromServices] AppDbContext context, [FromBody] RequestTreino request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower(), ct);
                if (usuario == null)
                {
                    Results.NotFound("Usuário ainda não foi registrado");
                }
                var treinos = await context.Treinos.Where(u => u.UsuarioId == usuario.Id).ToListAsync();
                if (treinos == null || treinos.Count == 0)
                {
                    return Results.NotFound("Nenhum treino encontrado para este usuário.");
                }
                return Results.Ok(treinos);
            });

            app.MapPost("/AtualizarCarga", async ([FromServices] AppDbContext context, [FromBody] RequestAtualizarCarga request, CancellationToken ct) =>
            {
                var treino = await context.Treinos.FirstOrDefaultAsync(t => t.UsuarioId == request.UsuarioId && t.Id == request.ExercicioId);
                if (treino == null)
                {
                    return Results.NotFound("Desculpe, Exercício não encontrado.");
                }

                treino.Series = request.Séries;
                treino.Carga = request.Carga;
                treino.Repeticoes = request.Repetições;
                
                await context.SaveChangesAsync(ct);
                return Results.Ok(new
                {
                    Exercício = treino.Exercicio,
                    NovaCarga = treino.Carga,
                    NovasRepetições = request.Repetições,
                    NovasSéries = request.Séries,
                });
            });

            app.MapGet("/RankingPerformance", async ([FromServices] AppDbContext context) =>
            {
                var ranking = await context.Treinos
                    .GroupBy(e => e.UsuarioId)
                    .Select(g => new
                    {
                        UsuarioId = g.Key,
                        TotalCarga = g.Sum(x => x.Carga * x.Repeticoes),
                        EvolucaoPercentual = (g.OrderByDescending(x => x.Data).First().Carga - g.OrderBy(x => x.Data).First().Carga)
                                             / g.OrderBy(x => x.Data).First().Carga * 100
                    })
                    .OrderByDescending(x => x.TotalCarga)
                    .ToListAsync();

                return Results.Ok(ranking);
            });
            app.MapGet("Teste", async ([FromServices] AppDbContext context) =>
            {
                var usuarios = await context.Usuario
                    .Where(a => a.Nome.ToLower() == "arthur")
        .           ToListAsync();
                return Results.Ok(usuarios);
            });

            app.MapPost("/VerTreino", async ([FromServices] AppDbContext context, RequestTreino request) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower());
                if (usuario == null)
                {
                    Results.NotFound("Usuário ainda não foi cadastrado");
                }
                var treinos = await context.Treinos
                    .Where(t => t.UsuarioId == usuario.Id)
                    .ToListAsync();

                if (treinos.Count == 0)
                {
                    return Results.NotFound("Nenhum treino encontrado para esse usuário.");
                }

                return Results.Ok(treinos);
            });
        }

        private static string CalculoCutting(Dieta dieta, Usuario usuario)
        {
            double tdee = usuario.TMB * usuario.FatorAtividade;
            dieta.CaloriaDieta = tdee * 0.80;

            dieta.Proteina = 2.2 * usuario.Peso;
            dieta.Gordura = 0.8 * usuario.Peso;
            dieta.Carboidrato = (dieta.CaloriaDieta - (dieta.Proteina * 4 + dieta.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {dieta.CaloriaDieta} / Proteinas totais: {dieta.Proteina} / Carboidratos totais: {dieta.Carboidrato} / Gorduras totais: {dieta.Gordura}";
            return resultadoCutting;
        }

        public static string CalculoBulking(Dieta dieta, Usuario usuario)
        {
            double tdee = usuario.TMB * usuario.FatorAtividade;
            dieta.CaloriaDieta = tdee * 1.15;
            dieta.Proteina = 1.8 * usuario.Peso;
            dieta.Gordura = 1 * usuario.Peso;
            dieta.Carboidrato = (dieta.CaloriaDieta - (dieta.Proteina * 4 + dieta.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {dieta.CaloriaDieta} / Proteinas totais: {dieta.Proteina} / Carboidratos totais: {dieta.Carboidrato} / Gorduras totais: {dieta.Gordura}";
            return resultadoCutting;
        }
        public static string CalculoManutencao(Dieta dieta, Usuario usuario)
        {
            dieta.CaloriaDieta = usuario.TMB * usuario.FatorAtividade;
            dieta.Proteina = 1.8 * usuario.Peso;
            dieta.Gordura = 0.9 * usuario.Peso;
            dieta.Carboidrato = (dieta.CaloriaDieta - (dieta.Proteina * 4 + dieta.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {dieta.CaloriaDieta} / Proteinas totais: {dieta.Proteina} / Carboidratos totais: {dieta.Carboidrato} / Gorduras totais: {dieta.Gordura}";
            return resultadoCutting;
        }

        private static double CalcularTMB(Usuario user)
        {
            // Fórmula Harris-Benedict
            double tbm;
            if (user.Sexo.Equals("Masculino", StringComparison.CurrentCultureIgnoreCase))
            {
                tbm = 88.36 + (13.4 * user.Peso) + (4.8 * user.Altura) - (5.7 * user.Idade);
            }
            else
            {
                tbm = 447.6 + (9.2 * user.Peso) + (3.1 * user.Altura) - (4.3 * user.Idade);
            }
            user.TMB = tbm;

            return user.TMB;
        }

        private static double CalcularGastoCalorico(Usuario user)
        {
            return user.TMB * user.FatorAtividade;
        }

        private static double ObterFatorAtividade(string nivelAtividadeFisica)
        {
            return nivelAtividadeFisica switch
            {
                "1" => 1.2,   // Sedentário
                "2" => 1.375, // Leve
                "3" => 1.55,  // Moderado
                "4" => 1.725, // Intenso
                "5" => 1.9,   // Muito Intenso
                _ => 1.0      // Valor padrão caso o nível informado seja inválido
            };
        }

      
    }
}
