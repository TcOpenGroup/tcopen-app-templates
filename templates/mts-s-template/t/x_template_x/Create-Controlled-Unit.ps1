Param( [Parameter(Mandatory=$true)]
       [string]$newCuName,
       [Parameter(Mandatory=$true , HelpMessage="Use CU00x as the default x_tempalte_x")]
       [string]$Cux_tempalte_xName )

function Copy-x_tempalte_x($newName)
{
    $x_tempalte_xFolder = $Cux_tempalte_xName
    Copy-Item  $x_tempalte_xFolder $newName -Recurse
}

function Rename-Files($newName)
{
    Get-ChildItem -File -Recurse | Rename-Item -NewName { $_.Name -Replace $Cux_tempalte_xName, $newName }
}

function Rename-Function-Blocks($newName)
{
    $files = Get-ChildItem -File -Recurse 
    foreach($file in $files)
    {
        $newContent = (Get-Content $file.FullName ).Replace($Cux_tempalte_xName,$newName) 
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
    $processDatax_tempalte_x = "`n`t`t$Cux_tempalte_xName : $Cux_tempalte_xNameProcessData := (Parent := THISSTRUCT);"
    $newProcessData = $processDatax_tempalte_x.Replace($Cux_tempalte_xName,$name)
    $startOfProcessDataRegion = (Get-Content $processDataDutPath | Select-String "END_STRUCT" ).LineNumber
    $processDataDUT = Get-Content $processDataDutPath
    $processDataDUT[$startOfProcessDataRegion -2] += $newProcessData
    Set-Content $processDataDutPath $processDataDUT
}

function Add-TechnologyData-Instance($name, $processDataDutPath)
{
    $techDatax_tempalte_x = "`n`t`t$Cux_tempalte_xName : $Cux_tempalte_xNameTechnologicalData := (Parent := THISSTRUCT);"
    $newProcessData = $techDatax_tempalte_x.Replace($Cux_tempalte_xName,$name)
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
    $x_tempalte_x =@"
    <Compile Include="Technology\PLACEHOLDER">
      <SubType>Code</SubType>
    </Compile>
"@
    $filesToAdd = Get-ChildItem $name -Recurse -File | Resolve-Path -Relative 
    $toAdd = ""
    foreach($file in $filesToAdd)
    {
        $toAdd += $x_tempalte_x.Replace("PLACEHOLDER", $file.Replace(".\","") )
    }
    $plcProjContent[$technologyNodeLineNumber-2] += $toAdd
    Set-Content $plcProjPath $plcProjContent
}

function x_tempalte_x-Exits($x_tempalte_xName)
{
    return (Test-Path $x_tempalte_xName)
}

function Create-New-Controlled-Unit($name)
{
   Push-Location ".\src\XAE\MainPlc\Technology"
   if (-Not (x_tempalte_x-Exits $Cux_tempalte_xName))
   {
    Write-Host "x_tempalte_x does not exits"
    Pop-Location
    return
   }
   Copy-x_tempalte_x $name
   Push-Location $name  
   Rename-Files $name
   Rename-Function-Blocks $name
   Remove-Tc-Id 
   Pop-Location
   Add-ProcessData-Instance $name ((Get-Item ".\Data\ProcessData.TcDUT").FullName)
   Add-TechnologyData-Instance $name ((Get-Item ".\Data\TechnologyData.TcDUT").FullName)
   Add-Cu-Folder $name ((Get-Item ".\..\MainPlc.plcproj").FullName)
   Link-Files-With-Project $name ((Get-Item ".\..\MainPlc.plcproj").FullName)
   Pop-Location
}


Create-New-Controlled-Unit $newCuName 
