UPDATE: If sources are pulled and run WITHOUT setting up user secrets in Visual Studio,
then the app will fail upon startup.

UPDATE: The HandleRequirementAsync() of the custom AuthorizationHandler, named SpecialFlagHandler,
is called twice. The first time the flow appears to work as designed. But the 2nd call may be
why this authorization model is failing. Hoping for some StackOverflow assistance.

Custom authentication plan: authentication with custom AuthorizationHandler that uses non-Identity 
database data to authenticate. 
Within my MongoDB there is a DB named AccessControl with a Collection named Pages.
Each Document in Pages has a pageName, representing an endpoint, and an array of ID's which are
Identity users' IDs as a string. Currently I manually add data to MongoDB to test the custom
authentication.

Example of Mongosh command to manually insert a Page Document:
db.Pages.insertOne({
  pageName: 'AuthFromProfileDB',
  allowedUsers: ['111b7cb643585fb3c20b9744','241b7cb999999fb3c20b9799','682265786d3715c85d6f0000']
});

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

