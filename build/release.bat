build\tools\MSBuild.exe app\Bump.sln /p:Configuration=Release

build\tools\ilmerge.exe /t:winexe /out:build\output\Bump.exe /ndebug app\Bump\Bump\bin\Release\Bump.exe app\Bump\Bump\bin\Release\Castle.Core.dll app\Bump\Bump\bin\Release\Simpler.dll app\Bump\Bump\bin\Release\Tasks.dll 

exit /B %ERRORLEVEL%
