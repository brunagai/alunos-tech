using System.Text;
using AlunosTech.Api.Auth;
using AlunosTech.Api.Health;
using AlunosTech.Api.Middlewares;
using AlunosTech.Application.UseCases;
using AlunosTech.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<MatricularAlunoUseCase>();
builder.Services.AddScoped<CriarChamadoSuporteUseCase>();
builder.Services.AddSingleton<EmissorJwt>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var chave = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(chave) || chave.Length < 32)
{
    throw new InvalidOperationException("Jwt:SigningKey precisa ter pelo menos 32 caracteres.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks()
    .AddCheck("processo", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<PostgresHealthCheck>("postgres", tags: ["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registro => registro.Tags.Contains("live")
}).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registro => registro.Tags.Contains("ready")
}).AllowAnonymous();

app.MapControllers();

app.Run();
