using AlunosTech.Infrastructure.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AlunosTech.Api.Health;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _escopos;

    public PostgresHealthCheck(IServiceScopeFactory escopos)
    {
        _escopos = escopos;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var escopo = _escopos.CreateAsyncScope();
            var banco = escopo.ServiceProvider.GetRequiredService<AlunosTechDbContext>();
            var conectou = await banco.Database.CanConnectAsync(cancellationToken);
            return conectou
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("PostgreSQL não aceitou a conexão.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL indisponível.", ex);
        }
    }
}
