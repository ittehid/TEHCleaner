$ErrorActionPreference = 'Stop'

$Root = Split-Path -Parent $PSScriptRoot
$Project = Join-Path $Root 'src\TEHCleaner\TEHCleaner.csproj'
$Output = Join-Path $Root 'artifacts\win-x64'

if (Test-Path $Output) {
    Remove-Item $Output -Recurse -Force
}
New-Item -ItemType Directory -Path $Output | Out-Null

dotnet restore $Project -r win-x64
dotnet publish $Project -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishTrimmed=false `
    -o $Output

$Exe = Join-Path $Output 'TEHCleaner.exe'
if (-not (Test-Path $Exe)) {
    throw 'TEHCleaner.exe was not produced.'
}

$Hash = Get-FileHash $Exe -Algorithm SHA256
$HashLine = "SHA256  $($Hash.Hash.ToLowerInvariant())  TEHCleaner.exe"
$HashLine | Set-Content (Join-Path $Output 'SHA256.txt') -Encoding ascii

Write-Host ''
Write-Host 'TEHCleaner release created:' -ForegroundColor Green
Write-Host $Exe
Write-Host $HashLine
