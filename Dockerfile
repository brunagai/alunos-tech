# syntax=docker/dockerfile:1

# Imagem da API Alunos Tech (.NET 8).
# Projeto: src/Api/AlunosTech.Api/AlunosTech.Api.csproj

FROM mcr.microsoft.com/dotnet/sdk:8.0.427 AS build
ARG PROJECT_FILE=src/Api/AlunosTech.Api/AlunosTech.Api.csproj
WORKDIR /src

COPY src/Domain/AlunosTech.Domain/AlunosTech.Domain.csproj src/Domain/AlunosTech.Domain/
COPY src/Application/AlunosTech.Application/AlunosTech.Application.csproj src/Application/AlunosTech.Application/
COPY src/Infrastructure/AlunosTech.Infrastructure/AlunosTech.Infrastructure.csproj src/Infrastructure/AlunosTech.Infrastructure/
COPY src/Api/AlunosTech.Api/AlunosTech.Api.csproj src/Api/AlunosTech.Api/
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "${PROJECT_FILE}"

COPY . .
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "${PROJECT_FILE}" \
      --configuration Release \
      --no-restore \
      --output /app/publish \
      /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0.33 AS runtime
WORKDIR /app

COPY --link --from=build --chown=1654:1654 /app/publish .
USER 1654

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080

LABEL org.opencontainers.image.title="Alunos Tech API" \
      org.opencontainers.image.description="API .NET 8 da plataforma de gestao de cursos e suporte corporativo"

ENTRYPOINT ["dotnet", "AlunosTech.Api.dll"]
