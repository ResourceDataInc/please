build\tools\MSBuild.exe app\Please.sln /p:Configuration=Release

build\tools\ilmerge.exe /targetplatform:v4,"%ProgramFiles%\Microsoft\Framework\.NETFramework\v4.0" /t:winexe /out:build\output\please.exe /ndebug app\Please\bin\Release\Please.exe app\Please\bin\Release\Castle.Core.dll app\Please\bin\Release\Simpler.dll app\Please\bin\Release\Library.dll 

exit /B %ERRORLEVEL%
