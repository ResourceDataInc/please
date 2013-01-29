build\tools\MSBuild.exe app\Please\Please.csproj /p:Configuration=Release

build\tools\ilmerge.exe /target:exe /out:build\output\please.exe /ndebug /targetplatform:v4 app\Please\bin\Release\Please.exe app\Please\bin\Release\Castle.Core.dll app\Please\bin\Release\Simpler.dll app\Please\bin\Release\Library.dll 

exit /B %ERRORLEVEL%
