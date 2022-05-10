param([Parameter(Mandatory=$true)]
      [string]$libraryName,
      [string]$baseDirectory)

$templateDirectory = (pwd).Path + '\t'
$baseDirectory = (pwd).Path 
$libraryDirectory = "$baseDirectory\$libraryName"
$templateString = "x_template_x"


if($libraryName -eq("t"))
{
    Write-Host "Name 't' is not permitted. Reserved for template directory."
    return
}

if($libraryName.Contains(".") -or $libraryName.Contains(" "))
{
    Write-Host "Library name cannot contain '.' or space ' ' "
    return
}



$existingDir = Get-ChildItem $baseDirectory -Directory -Filter $libraryName

if($existingDir.Name -eq $libraryName) 
{   
    Write-Host "Directory '"$libraryName"' already exists."
    return
}



#--------------------------------------------------------------------------
#                       Copy template files 
#--------------------------------------------------------------------------

mkdir $libraryDirectory 
$templateDirectory
$libraryDirectory

xcopy $templateDirectory $libraryDirectory /E

$files = Get-ChildItem $libraryDirectory -Exclude *bin*, *obj* -Recurse -File

#--------------------------------------------------------------------------
#                       Replace file content 
#--------------------------------------------------------------------------


Write-Host 'Replacing file content ' $files.Count

foreach ($file in $files)
{
        
    if($file.DirectoryName.Contains("\bin\") -or $file.DirectoryName.Contains("\obj\"))
    {
        continue;
    }

    $fileContent = Get-Content -Path $file.FullName

    if ($fileContent -match $templateString)
    {
        $newFileContent = $fileContent -replace $templateString, $libraryName
        Set-Content -Path $file.FullName -Value $newFileContent
    }
}

#--------------------------------------------------------------------------
#                       Change file names
#--------------------------------------------------------------------------

Write-Host 'Renaming file names'

foreach ($file in $files)
{
    if ($file -match $templateString)
    {
        $newName = $file.Name -replace $templateString, $libraryName
        Rename-Item -Path $file.FullName -NewName $newName
    }
}


#--------------------------------------------------------------------------
#                       Change diretory names
#--------------------------------------------------------------------------

Write-Host 'Renaming folder names'

$directories = Get-ChildItem $libraryDirectory -Directory -Recurse

$directories

[array]::Reverse($directories)

foreach ($directory in $directories)
{
    if ($directory -match $templateString)
    {
        $newName = $directory.Name -replace $templateString, $libraryName
        Rename-Item -Path $directory.FullName -NewName $newName
    }
}


#--------------------------------------------------------------------------
#                       Clean up
#--------------------------------------------------------------------------
Remove-Item "$libraryDirectory\scaffoldnewlibrary.ps1" -ErrorAction SilentlyContinue 

#--------------------------------------------------------------------------
#                      Build    
#--------------------------------------------------------------------------

dotnet build $libraryDirectory
$ivfrun = "$libraryDirectory\_Vortex\builder\vortex.compiler.console.exe"
& $ivfrun  -n false -s "$libraryDirectory\$libraryName.sln"
dotnet build $libraryDirectory

#--------------------------------------------------------------------------
#                      Done
#--------------------------------------------------------------------------
Write-Host "Scaffold for '" $libraryName "' created in " $templateDirectory\$libraryName
