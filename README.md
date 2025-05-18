UPDATE: If sources are pulled and run WITHOUT setting up user secrets in Visual Studio,
then the app will fail upon startup.

Authentication Plan: Customized AuthorizationHandler that uses a non-Identity 
database (named AccessControl) to correlate users and their access to the resources represented
by the AccessControl records. 

Upon initial startup this app will:
	1) add a root user to the Identity DB with name and password provided by ENV variables.
	2) create 2 roles in Identity DB: Admin and SuperAdmin
	3) give the root user both Admin and SuperAdmin roles
	4) create the AccessControl DB and add a record allowing the root user to
	   access the page (and it's components) referenced by @page directive "/AuthFromProfileDB""

This is a Blazor server web app that uses Identity and MongoDb.
This project will seed the MongoDB with a root user.
The root user's username and password must be defined as ENV variables:
 - SUPERUSERNAME
 - SUPERUSERPASSWORD
 
Initial project setup guidance: https://www.youtube.com/watch?v=asa2ucbZlCI&t=48s (CodeWrinkles)

Initial project setup steps:
 
Microsoft Visual Studio Community 2022 project template:
 - Blazor Web App - (choose .Net8)
 - Authentication type: Individual Accounts
 - Configure for HTTPS is checked
 - Interactive Render mode: Server
 - Interactivity location: Per page/component
 - Include sample pages is checked
 - Do not use top-level statements is checked
	
Then to remove unwanted stuff from project:
Delete from project:
 - Data->Migrations folder
 - Data->ApplicationDbContext.cs
   
From NuGet tools, UN-install packages:
 - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
 - Microsoft.AspNetCore.Identity.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.SqlServer
 - Microsoft.EntityFrameworkCore.Tools
	
From NuGet tools, Install packages:
	- AspNetCore.Identity.Mongo https://github.com/matteofabbri/AspNetCore.Identity.Mongo (by Matteo Fabbri)

