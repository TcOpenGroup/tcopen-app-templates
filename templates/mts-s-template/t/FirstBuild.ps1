# First build fails
dotnet build
cd _Vortex  
.\builder\vortex.compiler.console.exe
cd ..
dotnet build 