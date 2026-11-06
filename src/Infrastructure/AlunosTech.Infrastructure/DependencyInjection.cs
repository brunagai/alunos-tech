using AlunosTech.Application.Interfaces;
using AlunosTech.Infrastructure.Context;
using AlunosTech.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlunosTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<AlunosTechDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<IChamadoSuporteRepository, ChamadoSuporteRepository>();

        return services;
    }
}
