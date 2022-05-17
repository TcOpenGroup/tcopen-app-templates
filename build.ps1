$root = Get-Location
Set-Location templates\mts-s-template
./build.ps1
Set-Location $root
