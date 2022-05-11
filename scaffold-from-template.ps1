param([string]$Branch = "dev",
    [Parameter(Mandatory=$true)]    
    [string]$ProjectName,    
    [string]$TemplateId = "mts-s-template")

$baseDirectory = (Get-Location).Path    
$projectDirectory = "$baseDirectory\$projectName"

    
if($projectName.Contains(".") -or $projectName.Contains(" "))
{
        Write-Host "Library name cannot contain '.' or space ' ' "
        return
}

$existingDir = Get-ChildItem $baseDirectory -Directory -Filter $projectName

if($existingDir.Name -eq $projectName) 
{   
    Write-Host "Directory '"$projectName"' already exists."
    return
}

#--------------------------------------------------------------------------
#                   Downloading template repository
#--------------------------------------------------------------------------

Invoke-WebRequest https://github.com/TcOpenGroup/tcopen-app-templates/archive/refs/heads/$branch.zip -OutFile .\tcopen-app-templates-$branch.zip

#--------------------------------------------------------------------------
#                   Extracting template from repository
#--------------------------------------------------------------------------

$templateLocationInRepo = $baseDirectory + "\tcopen-app-templates-$branch\templates\$templateId\"
7z x tcopen-app-templates-$branch.zip "tcopen-app-templates-$branch/templates/$templateId/"

mkdir $projectName
$templateLocationInRepo
$projectDirectory
xcopy $templateLocationInRepo $projectDirectory /e

Remove-Item "tcopen-app-templates-$branch.zip" -Force -ErrorAction SilentlyContinue
Remove-Item "tcopen-app-templates-$branch" -Force -ErrorAction SilentlyContinue -Recurse

#--------------------------------------------------------------------------
#                 Scaffolding project from template
#--------------------------------------------------------------------------

$projectDirectory
Set-Location $projectDirectory
./scaffold $projectName

#--------------------------------------------------------------------------
#                           Cleaning up
#--------------------------------------------------------------------------
Remove-Item t -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item *.* -Force -ErrorAction SilentlyContinue



