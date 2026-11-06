$ErrorActionPreference = 'Stop'
$fase = Join-Path $PSScriptRoot '_fase.txt'
Set-Content -Path $fase -Value 'inicio' -Encoding ascii
Start-Sleep -Seconds 2
$raw = $Host.UI.RawUI
$raw.BufferSize = New-Object System.Management.Automation.Host.Size 200, 200
try { $raw.WindowSize = New-Object System.Management.Automation.Host.Size 180, 36 } catch { }
Write-Host 'psql \d ChamadosSuporte'
Get-Content -Raw (Join-Path $PSScriptRoot '_parte_chamados.sql') | kubectl --context kind-alunos-tech exec -i deploy/alunostech-db -- psql -U postgres -d AlunosTechDb -v ON_ERROR_STOP=1
Set-Content -Path $fase -Value 'chamados' -Encoding ascii
Start-Sleep -Seconds 16
Set-Content -Path $fase -Value 'fim' -Encoding ascii
