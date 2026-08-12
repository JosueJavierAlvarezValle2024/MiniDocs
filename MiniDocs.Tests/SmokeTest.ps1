$ErrorActionPreference = 'Stop'
$baseUrl = if ($env:MINIDOCS_BASE_URL) { $env:MINIDOCS_BASE_URL.TrimEnd('/') } else { 'http://localhost:5046' }

function Get-StatusCode([string]$url, [int]$expected) {
    try {
        $response = Invoke-WebRequest -UseBasicParsing -MaximumRedirection 0 $url
        $actual = [int]$response.StatusCode
    } catch {
        if ($_.Exception.Response) { $actual = [int]$_.Exception.Response.StatusCode }
        else { throw }
    }

    if ($actual -ne $expected) { throw "Falló $url. Esperado: $expected. Obtenido: $actual." }
    Write-Host "OK $url -> $actual"
}

Get-StatusCode "$baseUrl/" 200
Get-StatusCode "$baseUrl/Identity/Account/Login" 200
Get-StatusCode "$baseUrl/Usuarios" 302
Get-StatusCode "$baseUrl/Departamentos" 302
Get-StatusCode "$baseUrl/Documentos" 302
Write-Host 'Todas las pruebas HTTP pasaron.'
