param(
    [string]$RepositoryRoot = (Join-Path $PSScriptRoot '../..')
)

$ErrorActionPreference = 'Stop'
$secretPath = Join-Path $RepositoryRoot 'Plain Craft Launcher 2/Modules/ModSecret.vb'
$source = [IO.File]::ReadAllText($secretPath)

foreach ($credential in @(
    @{ Field = 'OAuthClientId'; Variable = 'PCL_MS_CLIENT_ID' },
    @{ Field = 'CurseForgeAPIKey'; Variable = 'PCL_CURSEFORGE_API_KEY' }
)) {
    $value = [Environment]::GetEnvironmentVariable($credential.Variable)
    if ([string]::IsNullOrEmpty($value)) { continue }
    if ($value.Contains("`r") -or $value.Contains("`n")) { throw 'API credentials must be single-line values.' }
    $escaped = $value.Replace('"', '""')
    $prefix = 'Public ' + $credential.Field + ' As String = If(Environment.GetEnvironmentVariable("' + $credential.Variable + '"), '
    $original = $prefix + '"")'
    if (-not $source.Contains($original)) { throw "Missing build placeholder for $($credential.Field)." }
    $source = $source.Replace($original, $prefix + '"' + $escaped + '")')
}

[IO.File]::WriteAllText($secretPath, $source, [Text.UTF8Encoding]::new($false))

if ($env:BUILD_COMMIT -notmatch '^[0-9a-f]{40}$') { throw 'BUILD_COMMIT must contain the full commit SHA.' }
$basePath = Join-Path $RepositoryRoot 'Plain Craft Launcher 2/Modules/Base/ModBase.vb'
$source = [IO.File]::ReadAllText($basePath)
$placeholder = 'Public Const CommitHash As String = ""'
if (-not $source.Contains($placeholder)) { throw 'Missing commit hash placeholder.' }
$source = $source.Replace($placeholder, 'Public Const CommitHash As String = "' + $env:BUILD_COMMIT + '"')
[IO.File]::WriteAllText($basePath, $source, [Text.UTF8Encoding]::new($false))
