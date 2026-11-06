$ErrorActionPreference = 'Stop'
$fase = Join-Path $PSScriptRoot '_fase.txt'
Set-Content -Path $fase -Value 'inicio' -Encoding ascii
Start-Sleep -Seconds 2

$raw = $Host.UI.RawUI
$raw.BufferSize = New-Object System.Management.Automation.Host.Size 200, 400
try {
    $raw.WindowSize = New-Object System.Management.Automation.Host.Size 180, 48
} catch {
    $raw.WindowSize = New-Object System.Management.Automation.Host.Size 140, 42
}

function Esperar([string]$nome) {
    Set-Content -Path $fase -Value $nome -Encoding ascii
    Start-Sleep -Seconds 16
    Clear-Host
}

Write-Host 'GET http://127.0.0.1:18080/health/live'
Write-Host (Invoke-WebRequest http://127.0.0.1:18080/health/live -UseBasicParsing).Content
Write-Host 'GET http://127.0.0.1:18080/health/ready'
Write-Host (Invoke-WebRequest http://127.0.0.1:18080/health/ready -UseBasicParsing).Content
Write-Host 'kubectl --context kind-alunos-tech get nodes'
kubectl --context kind-alunos-tech get nodes
Write-Host 'kubectl --context kind-alunos-tech get hpa'
kubectl --context kind-alunos-tech get hpa
Write-Host 'GET http://127.0.0.1:18080/swagger/index.html'
try {
    $swagger = Invoke-WebRequest http://127.0.0.1:18080/swagger/index.html -UseBasicParsing
    Write-Host $swagger.StatusCode
} catch {
    Write-Host ("HTTP " + [int]$_.Exception.Response.StatusCode)
}
Esperar 'sondas'

$sqlDir = $PSScriptRoot
function Psql([string]$nome) {
    Write-Host "psql -f $nome"
    Get-Content -Raw (Join-Path $sqlDir $nome) | kubectl --context kind-alunos-tech exec -i deploy/alunostech-db -- psql -U postgres -d AlunosTechDb -v ON_ERROR_STOP=1
}

Psql '_parte_alunos_cursos.sql'
Esperar 'alunos'
Psql '_parte_matriculas_chamados.sql'
Esperar 'matriculas'
Psql '_parte_lote.sql'
Esperar 'lote'
Set-Content -Path $fase -Value 'fim' -Encoding ascii
Start-Sleep -Seconds 8
