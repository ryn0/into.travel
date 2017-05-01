
----------
entity framework commands

Add-Migration -Name NAMEOFMIGRATION -ProjectName IntoTravel.Data -StartUpProjectName IntoTravel.Data 


Update-Database -Project IntoTravel.Data -StartUpProject IntoTravel.Data -Verbose
