@echo off

echo Into.Travel CI - Version 1.0.1
echo Copyright Into.Travel (tm) - All right reserved.

powershell -command "if (!(Get-Module psake -ListAvailable)) { if (!(Get-Module PsGet -ListAvailable)) { (New-Object Net.WebClient).DownloadString('http://psget.net/GetPsGet.ps1') | iex }; Install-Module psake }"

echo User Profile Path: %UserProfile%

if "%1" == "" (
	%UserProfile%\Documents\WindowsPowerShell\Modules\psake\psake.cmd .\CI\CI-Main.ps1 -framework 4.5.1 -docs -nologo
	%UserProfile%\Documents\WindowsPowerShell\Modules\psake\psake.cmd .\CI\CI-Main.ps1 -framework 4.5.1 ShowConfigs -nologo
) else (
	%UserProfile%\Documents\WindowsPowerShell\Modules\psake\psake.cmd .\CI\CI-Main.ps1 -framework 4.5.1 %* -nologo
)

