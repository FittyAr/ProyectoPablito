# build_installer.ps1
# Script para automatizar la publicacion y creacion del instalador de ElectroObraApp
# Autor: Antigravity

$ErrorActionPreference = "Stop"

$versionFile = ".\VERSION"
$projectPath = ".\ElectroObraApp.Desktop\ElectroObraApp.Desktop.csproj"
$publishDir = ".\publish_win"
$issPath = ".\installer_config.iss"
$isccPath = "C:\Program Files\Inno Setup 7\ISCC.exe"

# 0. Incrementar versión automáticamente
if (Test-Path $versionFile) {
    $currentVersion = Get-Content $versionFile
    Write-Host "Version actual: $currentVersion" -ForegroundColor Gray
    
    # Incrementar el último dígito
    if ($currentVersion -match '(\d+\.\d+\.)(\d+)') {
        $base = $Matches[1]
        $patch = [int]$Matches[2] + 1
        $newVersion = "$base$patch"
    } else {
        $newVersion = "$currentVersion.1"
    }
    
    $newVersion | Out-File $versionFile -Encoding utf8
    Write-Host "Nueva version generada: $newVersion" -ForegroundColor Cyan
} else {
    $newVersion = "1.0.0"
    $newVersion | Out-File $versionFile -Encoding utf8
    Write-Host "Archivo VERSION no encontrado. Iniciando en 1.0.0" -ForegroundColor Yellow
}

if (-not (Test-Path $isccPath)) {
    # Fallback a Inno Setup 6 o rutas comunes
    if (Test-Path "C:\Program Files (x86)\Inno Setup 6\ISCC.exe") {
        $isccPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    }
}

Write-Host "--- Iniciando proceso de construccion ---" -ForegroundColor Cyan

# 1. Limpiar carpeta de publicacion previa si existe
if (Test-Path $publishDir) {
    Write-Host "Limpiando carpeta de publicacion temporal..." -ForegroundColor Gray
    Remove-Item -Path $publishDir -Recurse -Force
}

# 2. Publicar el proyecto Avalonia Desktop
Write-Host "Publicando proyecto Avalonia Desktop (Windows x64)..." -ForegroundColor Yellow
dotnet publish $projectPath -f net10.0 -c Release -o $publishDir -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error durante la publicacion de .NET." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Compilar el instalador con Inno Setup
Write-Host "Compilando instalador con Inno Setup (Version $newVersion)..." -ForegroundColor Yellow

if (Test-Path $isccPath) {
    & $isccPath $issPath "/DAppVersion=$newVersion"
} else {
    Write-Host "Error: No se encontro ISCC.exe. Por favor verifica la ruta de Inno Setup." -ForegroundColor Red
    Write-Host "Ruta intentada: $isccPath" -ForegroundColor Gray
    exit 1
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error durante la compilacion del instalador." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 4. Limpieza de archivos temporales
Write-Host "Eliminando archivos temporales de publicacion..." -ForegroundColor Gray
Remove-Item -Path $publishDir -Recurse -Force

Write-Host "Proceso completado con exito! El instalador esta listo en la carpeta Output." -ForegroundColor Green
