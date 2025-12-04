param(
    [Parameter(Mandatory=$true)]
    [string]$NewVersion,  # 例: "1.0.1"

    [Parameter(Mandatory=$false)]
    [switch]$PublishToGitHub,  # GitHub Packagesに公開するか

    [Parameter(Mandatory=$false)]
    [switch]$SkipLocal  # ローカルフィードへのコピーをスキップ
)

$kdxProjectsRoot = $PSScriptRoot

Write-Host "Updating KdxProjects to version $NewVersion..." -ForegroundColor Green

# 1. Directory.Build.propsを更新
$buildPropsPath = Join-Path $kdxProjectsRoot "Directory.Build.props"
Write-Host "Updating Directory.Build.props..." -ForegroundColor Yellow

$buildPropsContent = Get-Content $buildPropsPath -Raw
$buildPropsContent = $buildPropsContent -replace '<Version>[\d\.]+</Version>', "<Version>$NewVersion</Version>"
$buildPropsContent = $buildPropsContent -replace '<AssemblyVersion>[\d\.]+\.0</AssemblyVersion>', "<AssemblyVersion>$NewVersion.0</AssemblyVersion>"
$buildPropsContent = $buildPropsContent -replace '<FileVersion>[\d\.]+\.0</FileVersion>', "<FileVersion>$NewVersion.0</FileVersion>"
Set-Content -Path $buildPropsPath -Value $buildPropsContent
Write-Host "✓ Updated Directory.Build.props" -ForegroundColor Cyan

# 2. Releaseビルド
Set-Location $kdxProjectsRoot
Write-Host "`nBuilding Release..." -ForegroundColor Yellow
dotnet build -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build succeeded" -ForegroundColor Cyan

# 3. NuGetパッケージをローカルフィードにコピー（スキップしない場合）
if (-not $SkipLocal) {
    Write-Host "`nCopying packages to local NuGet feed..." -ForegroundColor Yellow

    # ローカルフィードディレクトリを作成（存在しない場合）
    $localFeed = "C:\NuGetLocal"
    if (-not (Test-Path $localFeed)) {
        New-Item -ItemType Directory -Path $localFeed -Force | Out-Null
    }

    Copy-Item -Path "$kdxProjectsRoot\src\Kdx.Contracts\bin\Release\*.nupkg" -Destination $localFeed -Force
    Copy-Item -Path "$kdxProjectsRoot\src\Kdx.Core\bin\Release\*.nupkg" -Destination $localFeed -Force
    Copy-Item -Path "$kdxProjectsRoot\src\Kdx.Infrastructure\bin\Release\*.nupkg" -Destination $localFeed -Force
    Copy-Item -Path "$kdxProjectsRoot\src\Kdx.Infrastructure.Supabase\bin\Release\*.nupkg" -Destination $localFeed -Force
    Copy-Item -Path "$kdxProjectsRoot\src\Kdx.Contracts.ViewModels\bin\Release\*.nupkg" -Destination $localFeed -Force

    Write-Host "✓ Packages copied to $localFeed" -ForegroundColor Cyan
} else {
    Write-Host "`nSkipping local NuGet feed copy..." -ForegroundColor Yellow
}

# 4. 生成されたパッケージを表示
if (-not $SkipLocal) {
    Write-Host "`n📦 Generated packages:" -ForegroundColor Green
    Get-ChildItem "$localFeed\*.nupkg" | Where-Object { $_.Name -like "*$NewVersion*" } | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor White
    }
}

# 5. GitHub Packagesに公開
if ($PublishToGitHub) {
    Write-Host "`n📦 Publishing to GitHub Packages..." -ForegroundColor Green

    # GitHub認証トークンを環境変数から取得
    $githubToken = $env:GITHUB_PACKAGES_TOKEN
    if (-not $githubToken) {
        Write-Host "✗ GITHUB_PACKAGES_TOKEN environment variable not set!" -ForegroundColor Red
        Write-Host "Run: `$env:GITHUB_PACKAGES_TOKEN = 'your-token'" -ForegroundColor Yellow
        exit 1
    }

    # GitHub Packagesソースを追加（まだなければ）
    Write-Host "  Adding GitHub Packages source..." -ForegroundColor Cyan
    dotnet nuget add source https://nuget.pkg.github.com/KANAMORI-SYSTEM-Inc/index.json `
        --name KdxGitHub `
        --username KANAMORI-SYSTEM-Inc `
        --password $githubToken `
        --store-password-in-clear-text `
        2>$null

    # パッケージを公開
    $publishedCount = 0
    Get-ChildItem "$kdxProjectsRoot\src\*\bin\Release\*.nupkg" | Where-Object { $_.Name -notlike "*.symbols.nupkg" } | ForEach-Object {
        Write-Host "  Publishing $($_.Name)..." -ForegroundColor Cyan
        dotnet nuget push $_.FullName --source KdxGitHub --api-key $githubToken --skip-duplicate
        if ($LASTEXITCODE -eq 0) {
            $publishedCount++
        }
    }

    if ($publishedCount -gt 0) {
        Write-Host "✓ Published $publishedCount package(s) to GitHub Packages" -ForegroundColor Green
    } else {
        Write-Host "⚠ No packages were published (might already exist)" -ForegroundColor Yellow
    }
}

Write-Host "`n✓ KdxProjects updated to $NewVersion successfully!" -ForegroundColor Green
Write-Host "`n📝 Next steps:" -ForegroundColor Yellow
if (-not $PublishToGitHub) {
    Write-Host "  1. Update CHANGELOG.md manually"
    Write-Host "  2. Update KdxDesigner:"
    Write-Host "     cd C:\Users\amdet\source\repos\KANAMORI-SYSTEM-Inc\KdxDesigner"
    Write-Host "     .\update-kdxdesigner.ps1 -NewVersion $NewVersion"
    Write-Host "  3. Commit and tag:"
    Write-Host "     git add ."
    Write-Host "     git commit -m 'Release v$NewVersion'"
    Write-Host "     git tag -a v$NewVersion -m 'Release v$NewVersion'"
    Write-Host "     git push origin master --tags"
} else {
    Write-Host "  1. Update CHANGELOG.md manually"
    Write-Host "  2. Commit and push to GitHub:"
    Write-Host "     git add ."
    Write-Host "     git commit -m 'Release v$NewVersion'"
    Write-Host "     git tag -a v$NewVersion -m 'Release v$NewVersion'"
    Write-Host "     git push origin master --tags"
    Write-Host "  3. Update KdxDesigner:"
    Write-Host "     cd C:\Users\amdet\source\repos\KANAMORI-SYSTEM-Inc\KdxDesigner"
    Write-Host "     .\update-kdxdesigner.ps1 -NewVersion $NewVersion -Source GitHub"
}
