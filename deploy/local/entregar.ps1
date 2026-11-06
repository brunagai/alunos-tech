# Entrega no cluster kind local. Não usa registro público nem GitHub Actions.
$ErrorActionPreference = "Stop"
$raiz = Resolve-Path (Join-Path $PSScriptRoot "..\..")
Set-Location $raiz

$tag = (git rev-parse --short HEAD).Trim()
$imagem = "localhost:5001/alunos-tech-api:$tag"
Write-Output "IMAGEM=$imagem"

dotnet publish "src/Api/AlunosTech.Api/AlunosTech.Api.csproj" --configuration Release --output artifacts/api-publish
if ($LASTEXITCODE -ne 0) { throw "dotnet publish falhou com $LASTEXITCODE" }

docker build -f Dockerfile.cluster -t $imagem artifacts/api-publish
if ($LASTEXITCODE -ne 0) { throw "docker build falhou com $LASTEXITCODE" }

docker push $imagem
if ($LASTEXITCODE -ne 0) { throw "docker push falhou com $LASTEXITCODE" }

kubectl set image deployment/alunostech-api api=$imagem
if ($LASTEXITCODE -ne 0) { throw "kubectl set image falhou com $LASTEXITCODE" }

kubectl rollout status deployment/alunostech-api --timeout=180s
if ($LASTEXITCODE -ne 0) { throw "rollout falhou com $LASTEXITCODE" }

kubectl get deployment alunostech-api -o jsonpath="{.spec.template.spec.containers[0].image}"
Write-Output ""
