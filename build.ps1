$root = Get-Location
Set-Location .\templates\mts-s-template\
./build.ps1
Set-Location $root

Set-Location .\scaffolder\TcOpen.Scaffold\
./build.ps1
Set-Location $root
