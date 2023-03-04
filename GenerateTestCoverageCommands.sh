dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --collect "Xplat Code Coverage" &&
    reportgenerator "-reports:./tests/TestResults/**/**/*.xml" "-targetdir:./tests/CodeCoverageReports/" -reportTypes:html
