Into.Travel
-----
Into.Travel is a blogging site for travel photography. It was written in C# and runs on .NET Core.


Deployment
-----
You will need to add a SQL Server and Azure Storage connection string to the appsettings.json file for the web and data projects. Both should have the same values.

The website and database can be deployed using the CI.bat file which needs parameters for the IIS server it should be deployed to. 


How to add a database migration
-----
entity framework commands

Add-Migration -Name NAMEOFMIGRATION -Project IntoTravel.Data -StartUpProject IntoTravel.Data 

Update-Database -Project IntoTravel.Data -StartUpProject IntoTravel.Data -Verbose


Features
-----
-Create/ edit blog entries with preview mode that has images and rich text editing on each
-Manage static files for use in the site 
-All static files uploaded work with a CDN for high speed
-Tagging of blogs for filtering
-Email subscribe to collect emails
-Sitemap of blog posts for Google
-Create/ edit snippets of text for different site sections
-Create/ edit link redirections (a link shortener)
-Login system for single admin