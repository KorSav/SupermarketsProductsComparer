$ErrorActionPreference = "Stop"

# Move working directory to project root: one level above the script location
$ProjectRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Push-Location $ProjectRoot

try {
    dotnet build
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed." }

    dotnet-coverage collect `
        -s "sonarqube/coverage.runsettings" `
        "dotnet test --no-build" `
        -f xml `
        -o "sonarqube/coverage.xml"
    if ($LASTEXITCODE -ne 0) { throw "dotnet-coverage collect failed." }

    reportgenerator `
        -reports:"sonarqube/coverage.xml" `
        -targetdir:"sonarqube/report" `
        -reporttypes:HtmlInline_AzurePipelines
    if ($LASTEXITCODE -ne 0) { throw "ReportGenerator failed." }
}
finally {
    Pop-Location
}