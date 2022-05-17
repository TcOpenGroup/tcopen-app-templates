# dotnet run --project cake/Build.csproj -- $args
# We need to use msbuild instead of dotnet due to COMrefs in the project.

$msbuild =([System.Environment]::GetEnvironmentVariable('TcoMsbuild'))
cd cake/
& $msbuild cake/Build.csproj /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU /p:TargetFramework=net5.0 /p:DefineConstants=Release $args
.\bin\Release\net5.0\Build.exe $args    
cd ..
exit $LASTEXITCODE;