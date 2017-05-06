# External scripts
$CIRoot              = $PSScriptRoot

# Define CLI input properties and defaults
properties {

   # Build
   $BuildConfiguration          = "release"
   $DotNetRunTime               = "win7-x64"
   $DotNetFramework             = "netcoreapp1.1"
   $msDeploy                    = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"    
   
   # Project paths
   $databaseProjectSourcePath   = "..\src\IntoTravel.Data"
   $webProjectSourcePath        = "..\src\IntoTravel.Web"
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

task -name DeployWebApp -depends RestorePackages, BuildProject, MigrateDB -action {

    exec {

        if (!(Test-Path -Path ("$CIRoot\$compileSourcePath")))
        {
            New-Item -Path ("$CIRoot\$compileSourcePath") -ItemType directory
        }

        $compileSourcePath = Resolve-Path -Path ("$CIRoot\$compileSourcePath")

        $webProjectPath = Resolve-Path -Path ("$CIRoot\$webProjectSourcePath")

        cd $webProjectPath
    
        & dotnet publish `
                    --framework $DotNetFramework `
                    --output $compileSourcePath `
                    --configuration $BuildConfiguration `
                    --runtime $DotNetRunTime
 
        $webconfigPath = $contentPathDes + "web.config"
        $deployIisAppPath = $webAppHost
        
        Write-Host "Deleting config..."
        & $msDeploy `
            -verb:delete `
            -allowUntrusted:true `
            -dest:contentPath=$webconfigPath,computername=$MsDeployLocation/MsDeploy.axd?site=$deployIisAppPath,username=$msDeployUserName,password=$msDeployPassword,authtype=basic
        Write-Host "done."
       
        Write-Host "Deploying..."
        & $msDeploy `
            -verb:sync `
            -source:contentPath=$compileSourcePath `
            -allowUntrusted:true `
            -dest:contentPath=$contentPathDes,computername=$MsDeployLocation/MsDeploy.axd?site=$deployIisAppPath,username=$msDeployUserName,password=$msDeployPassword,authtype=basic
        Write-Host "done."
         
        $url = "http://$webAppHost"
        Write-Host "Deployment completed, requesting page '$url'...." -NoNewline 
        Invoke-WebRequest -Uri $url | out-null
        Write-Host "done." -NoNewline
        Write-Host
            
        Write-Host "COMPLETE!"
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

        dotnet build
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