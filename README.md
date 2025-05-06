UPDATE: as of May2,2025 commits, if sources are pulled and run WITHOUT setting up user secrets in Visual Studio,
then the app will fail upon startup.

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

