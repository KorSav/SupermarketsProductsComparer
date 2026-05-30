$ErrorActionPreference = "Stop"

# Move working directory to project root: one level above the script location
$ProjectRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Push-Location $ProjectRoot

try {
    $SonarToken = "sqp_07b8898c97e0d4bbff21cb656c33211e38834f2d"

    dotnet clean
    if ($LASTEXITCODE -ne 0) { throw "dotnet clean failed." }

    dotnet sonarscanner begin `
        /k:"ProductPriceMonitoring" `
        /d:sonar.host.url="http://localhost:9000" `
        /d:sonar.token="$SonarToken" `
        /d:sonar.exclusions="**/Migrations/**,**/bin/**,**/obj/**,**/wwwroot/**,**/sonarqube/**" `
        /d:sonar.cs.vscoveragexml.reportsPaths="sonarqube/coverage.xml" `
        /d:sonar.dotnet.excludeTestProjects=true

    if ($LASTEXITCODE -ne 0) { throw "SonarScanner begin failed." }

    dotnet build --no-incremental
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed." }

    dotnet-coverage collect `
        -s "sonarqube/coverage.runsettings" `
        "dotnet test --no-build" `
        -f xml `
        -o "sonarqube/coverage.xml"

    if ($LASTEXITCODE -ne 0) { throw "dotnet-coverage collect failed." }

    dotnet sonarscanner end /d:sonar.token="$SonarToken"
    if ($LASTEXITCODE -ne 0) { throw "SonarScanner end failed." }

    reportgenerator `
        -reports:"sonarqube/coverage.xml" `
        -targetdir:"sonarqube/report" `
        -reporttypes:HtmlInline_AzurePipelines

    if ($LASTEXITCODE -ne 0) { throw "ReportGenerator failed." }
}
finally {
    Pop-Location
}