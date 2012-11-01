build\tools\MSBuild.exe app\Bump.sln /p:Configuration=Release

build\tools\ilmerge.exe /t:winexe /out:build\output\Bump.exe /ndebug app\Bump\bin\Release\Bump.exe app\Bump\bin\Release\Castle.Core.dll app\Bump\bin\Release\Simpler.dll app\Bump\bin\Release\Library.dll 

exit /B %ERRORLEVEL%
