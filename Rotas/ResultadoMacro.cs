using Microsoft.EntityFrameworkCore;
using MuscleMetrics.Data;
using MuscleMetrics.Entidades;

namespace MuscleMetrics.Rotas
{
    public static class ResultadoMacro
    {
        public static void AddRotasResultadoMacro(this WebApplication app)
        {
            // Endpoint para calcular a TMB e armazenar no banco
            app.MapPost("/CalcularTaxaMetabolicaBasal", async (AppDbContext context, RequestTBM request, CancellationToken ct) =>
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
            app.MapPost("/CalcularGastoCalorico", async (AppDbContext context, RequestGastoCalorico request, CancellationToken ct) =>
            {
                var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower() == request.Nome.ToLower());
                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                usuario.FatorAtividade = ObterFatorAtividade(request.NivelAtividadeFisica);
                usuario.GastoCalórico = usuario.TMB * usuario.FatorAtividade;
                await context.SaveChangesAsync(ct);

                return Results.Ok(new
                {
                    Usuario = usuario.Nome,
                    Gasto_Calórico = usuario.GastoCalórico 
                });
            });

            app.MapPost("/EscolhaSeuObjetivo", async (AppDbContext context, RequestObjetivo request, CancellationToken ct) =>
            { 
               var usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Nome.ToLower().Equals(request.Nome.ToLower()));
                if (usuario == null)
                {
                    return Results.NotFound("Usuário não encontrado.");
                }

                if (request.Objetivo.Equals("Cutting", StringComparison.InvariantCultureIgnoreCase))
                {
                    CalculoCutting(usuario);
                    await context.SaveChangesAsync(ct);
                    return Results.Ok(new
                    {
                        Usuario = usuario.Nome,
                         DietaResultado = CalculoCutting(usuario)
                    });
                }
                else if (request.Objetivo.Equals("Bulking", StringComparison.InvariantCultureIgnoreCase))
                {
                    CalculoBulking(usuario);
                    await context.SaveChangesAsync(ct);
                    return Results.Ok(new
                    {
                        Usuario = usuario.Nome,
                        DietaResultado = CalculoBulking(usuario)
                    });
                }
                else if (request.Objetivo.Equals("Manutenção", StringComparison.InvariantCultureIgnoreCase))
                {
                    CalculoManutencao(usuario);
                    await context.SaveChangesAsync(ct);
                    return Results.Ok(new
                    {
                        Usuario = usuario.Nome,
                        DietaResultado = CalculoManutencao(usuario)
                    });
                } 
                else
                {
                    return Results.NotFound("Objetivo não identificado...Os únicos possíveis são:  Bulking / Cutting / Manutenção ");
                }                            
            });
        }

        private static string CalculoCutting(Usuario usuario)
        {
             double tdee = usuario.TMB * usuario.FatorAtividade;
             usuario.CaloriaDieta = tdee * 0.80; 

             usuario.Proteina = 2.2 * usuario.Peso;
             usuario.Gordura = 0.8 * usuario.Peso;
             usuario.Carboidrato = (usuario.CaloriaDieta - (usuario.Proteina * 4 + usuario.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {usuario.CaloriaDieta} / Proteinas totais: {usuario.Proteina} / Carboidratos totais: {usuario.Carboidrato} / Gorduras totais: {usuario.Gordura}";
            return resultadoCutting;
        }

        public static string CalculoBulking(Usuario usuario)
        {
            double tdee = usuario.TMB * usuario.FatorAtividade;
            usuario.CaloriaDieta = tdee * 1.15;

            usuario.Proteina = 1.8 * usuario.Peso;
            usuario.Gordura = 1 * usuario.Peso;
            usuario.Carboidrato = (usuario.CaloriaDieta - (usuario.Proteina * 4 + usuario.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {usuario.CaloriaDieta} / Proteinas totais: {usuario.Proteina} / Carboidratos totais: {usuario.Carboidrato} / Gorduras totais: {usuario.Gordura}";
            return resultadoCutting;
        }
        public static string CalculoManutencao(Usuario usuario)
        {
            usuario.CaloriaDieta = usuario.TMB * usuario.FatorAtividade;

            usuario.Proteina = 1.8 * usuario.Peso;
            usuario.Gordura = 0.9 * usuario.Peso;
            usuario.Carboidrato = (usuario.CaloriaDieta - (usuario.Proteina * 4 + usuario.Gordura * 9)) / 4;
            string resultadoCutting = $"(Dieta para 4 refeições ao dia) Calorias totais: {usuario.CaloriaDieta} / Proteinas totais: {usuario.Proteina} / Carboidratos totais: {usuario.Carboidrato} / Gorduras totais: {usuario.Gordura}";
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