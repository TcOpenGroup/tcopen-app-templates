Param( [Parameter(Mandatory=$true)]
       [string]$newCuName,
       [Parameter(Mandatory=$true , HelpMessage="Use CU00x as the default Template")]
       [string]$CuTemplateName )

function Copy-Template($newName)
{
    $TemplateFolder = $CuTemplateName
    Copy-Item  $TemplateFolder $newName -Recurse
}

function Rename-Files($newName)
{
    Get-ChildItem -File -Recurse | Rename-Item -NewName { $_.Name -Replace $CuTemplateName, $newName }
}

function Rename-Function-Blocks($newName)
{
    $files = Get-ChildItem -File -Recurse 
    foreach($file in $files)
    {
        $newContent = (Get-Content $file.FullName ).Replace($CuTemplateName,$newName) 
        Set-Content $file.FullName $newContent
    }
}

function Remove-Tc-Id
{
    $files = Get-ChildItem -File -Recurse 
    foreach($file in $files)
    {
        $regex = 'Id=\"{.+}\"'

        $newContent = (Get-Content $file.FullName ) -Replace($regex,"") 
        Set-Content $file.FullName $newContent
    }
}

function Add-ProcessData-Instance($name, $processDataDutPath)
{
    $processDataTemplate = "`n`t`t$CuTemplateName : $CuTemplate"+"NameProcessData := (Parent := THISSTRUCT);"
    $newProcessData = $processDataTemplate.Replace($CuTemplateName,$name)
    $startOfProcessDataRegion = (Get-Content $processDataDutPath | Select-String "END_STRUCT" ).LineNumber
    $processDataDUT = Get-Content $processDataDutPath
    $processDataDUT[$startOfProcessDataRegion -2] += $newProcessData
    Set-Content $processDataDutPath $processDataDUT
}

function Add-TechnologyData-Instance($name, $processDataDutPath)
{
    $techDataTemplate = "`n`t`t$CuTemplateName : $CuTemplateName"+"TechnologicalData := (Parent := THISSTRUCT);"
    $newProcessData = $techDataTemplate.Replace($CuTemplateName,$name)
    $startOfProcessDataRegion = (Get-Content $processDataDutPath | Select-String "END_STRUCT" ).LineNumber
    $processDataDUT = Get-Content $processDataDutPath
    $processDataDUT[$startOfProcessDataRegion -2] += $newProcessData
    Set-Content $processDataDutPath $processDataDUT
}

function Add-Cu-Folder($name, $plcProjPath)
{
    $plcProjContent = Get-Content $plcProjPath
    $folderDefinition = '<Folder Include="Technology\PLACEHOLDER" />'.Replace("PLACEHOLDER",$name)    
    $alreadyExists =  $plcProjContent | Select-String $folderDefinition -SimpleMatch
    if($alreadyExists)
    {
        Write-Host 'Folder exists'
        return
    }
    $technologyFolderLineNumer = ($plcProjContent | Select-String '<Folder Include="Technology" />').LineNumber

    $plcProjContent[$technologyFolderLineNumer+1] += $folderDefinition
    Set-Content $plcProjPath $plcProjContent
}

function Link-Files-With-Project($name, $plcProjPath)
{
    $plcProjContent = Get-Content $plcProjPath
    $technologyNode = '<Compile Include="Technology\Technology.TcPOU">'   
    $technologyNodeLineNumber = ($plcProjContent | Select-String $technologyNode -SimpleMatch).LineNumber
    $Template =@"
    <Compile Include="Technology\PLACEHOLDER">
      <SubType>Code</SubType>
    </Compile>
"@
    $filesToAdd = Get-ChildItem $name -Recurse -File | Resolve-Path -Relative 
    $toAdd = ""
    foreach($file in $filesToAdd)
    {
        $toAdd += $Template.Replace("PLACEHOLDER", $file.Replace(".\","") )
    }
    $plcProjContent[$technologyNodeLineNumber-2] += $toAdd
    Set-Content $plcProjPath $plcProjContent
}

function Template-Exits($TemplateName)
{
    return (Test-Path $TemplateName)
}

function Create-New-Controlled-Unit($name)
{
   Push-Location ".\src\x_template_x-xae\x_template_xPlc\Technology"
   if (-Not (Template-Exits $CuTemplateName))
   {
    Write-Host "Template does not exits"
    Pop-Location
    return
   }
   Copy-Template $name
   Push-Location $name  
   Rename-Files $name
   Rename-Function-Blocks $name
   Remove-Tc-Id 
   Pop-Location
   Add-ProcessData-Instance $name ((Get-Item ".\Data\ProcessData.TcDUT").FullName)
   Add-TechnologyData-Instance $name ((Get-Item ".\Data\TechnologyData.TcDUT").FullName)
   Add-Cu-Folder $name ((Get-Item ".\..\x_template_xPlc.plcproj").FullName)
   Link-Files-With-Project $name ((Get-Item ".\..\x_template_xPlc.plcproj").FullName)
   Pop-Location
}


Create-New-Controlled-Unit $newCuName 
