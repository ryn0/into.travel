﻿# External scripts
$CIRoot              = $PSScriptRoot

# Define CLI input properties and defaults
properties {

   # Build
   $BuildConfiguration          = "release"
   $DotNetRunTime               = "win7-x64"
   $DotNetFramework             = "netcoreapp2.0"
   $msDeploy                    = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"    
   
   # Project paths
   $databaseProjectSourcePath   = "..\src\IntoTravel.Data"
   $webProjectSourcePath        = "..\src\IntoTravel.Web"
   $testProjectSourcePath        = "..\IntoTravel.Core.UnitTests"
   $compileSourcePath           = "..\src\IntoTravel.Web\bin\output"

   # Credentials
   $MsDeployLocation            = ""
   $webAppHost                  = ""
   $contentPathDes              = ""
   $msDeployUserName            = ""
   $msDeployPassword            = ""
}

task default -depends RestorePackages # required task

##############
# User Tasks
##############

task -name CreatePackage {

    exec {

        if (!(Test-Path -Path ("$CIRoot\$compileSourcePath")))
        {
            New-Item -Path ("$CIRoot\$compileSourcePath") -ItemType directory
        }

        $compileSourcePath = Resolve-Path -Path ("$CIRoot\$compileSourcePath")

        $webProjectPath = Resolve-Path -Path ("$CIRoot\$webProjectSourcePath")

        cd $webProjectPath

        Write-Host "Packaging..."
    
        & dotnet publish `
                    --framework $DotNetFramework `
                    --output $compileSourcePath `
                    --configuration $BuildConfiguration `
                    --runtime $DotNetRunTime
    }
}

task -name RunUnitTests {

    exec {
        $path = Resolve-Path -Path $testProjectSourcePath
        Write-Host "Test project: $path"
        dotnet test $path
    }
}

task -name SyncWebFiles {

    exec {

        $webconfigPath = $contentPathDes + "web.config"
        $deployIisAppPath = $webAppHost
        
        Write-Host "Deleting config..."
        
        & $msDeploy `
            -verb:delete `
            -allowUntrusted:true `
            -dest:contentPath=$webconfigPath,computername=$MsDeployLocation/MsDeploy.axd?site=$deployIisAppPath,username=$msDeployUserName,password=$msDeployPassword,authtype=basic
        Write-Host "done."

        $compileSourcePath = Resolve-Path -Path ("$CIRoot\$compileSourcePath")

        Write-Host "-------------"
        Write-Host "Deploying..."

        & $msDeploy `
            -verb:sync `
            -source:contentPath=$compileSourcePath `
            -allowUntrusted:true `
            -dest:contentPath=$contentPathDes,computername=$MsDeployLocation/MsDeploy.axd?site=$deployIisAppPath,username=$msDeployUserName,password=$msDeployPassword,authtype=basic
        Write-Host "done."
    }
}

task -name DeployWebApp -depends RestorePackages, BuildProject, RunUnitTests, MigrateDB, CreatePackage, SyncWebFiles -action {

    exec {

        $url = "http://$webAppHost"
        Write-Host "Deployment completed, requesting page '$url'..." -NoNewline 
        
        $response = Invoke-WebRequest -Uri $url

        if ($response.StatusCode -eq 200)
        {
            Write-Host "done." -NoNewline
            Write-Host
                
            Write-Host "COMPLETE!"
        }
        else 
        {
            Write-Error "Status code was: " + $response.StatusCode
        }
    }
}

task -name BuildProject -description "Build Project"  -action { 
   
     exec {
     
        $webProjectPath = Resolve-Path -Path ("$CIRoot\$webProjectSourcePath")

        cd $webProjectPath

        dotnet restore
    }
}

task -name RestorePackages -description "Restore Packages"  -action { 
   
     exec {

        $compileSourcePath = ("$CIRoot\$compileSourcePath")
        
        if (Test-Path -Path $compileSourcePath){
            Write-Host "Deleting files at: '$compileSourcePath'... " -NoNewline
            
            Remove-Item $compileSourcePath -Force -Recurse

            Write-Host "done." -NoNewline
            Write-host
        }
                            
        $webProjectPath = Resolve-Path -Path ("$CIRoot\$webProjectSourcePath")

        cd $webProjectPath

        
        dotnet msbuild /t:Restore /p:Configuration=$BuildConfiguration

    }
}

task -name MigrateDB -description "Runs migration of database"  -action { 
   
     exec {
      
       $databaseProjectSourcePath = Resolve-Path -Path ("$CIRoot\$databaseProjectSourcePath")
       cd $databaseProjectSourcePath
       Write-Host $databaseProjectSourcePath

       dotnet ef database update --verbose
    }
}

task -name Push -description "Pushes source" -action {

    exec {
        git push origin master
    }
}

task -name RenameFiles -description "Renames files in a directory" {

    $path = "c:\temp"
    $fileNamSubstringeToRemove = "-min"

    Get-ChildItem  -Path $path | Rename-Item -NewName { $_.Name -replace $fileNamSubstringeToRemove, '' }
}

################
# Psake Helpers
################

# Runs automatically before each task.                             
TaskSetup {
    if ($displayTaskStartTime)
    {
        $currentTaskTime = (Get-date).ToUniversalTime().ToString("u")
        Write-Host "Started at: $currentTaskTime" -ForegroundColor DarkGray
    }
}

# Runs automatically after each task.     
TaskTearDown {
	if ($LASTEXITCODE -ne $null -and $LASTEXITCODE -ne 0) {
        $message = "Task failed with exit code '$LASTEXITCODE', see previous errors for more information."

		throw $message
	}

	""
}