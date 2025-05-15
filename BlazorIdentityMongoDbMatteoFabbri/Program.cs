using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo;
using BlazorIdentityMongoDbMatteoFabbri.Components;
using BlazorIdentityMongoDbMatteoFabbri.Components.Account;
using BlazorIdentityMongoDbMatteoFabbri.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using BlazorIdentityMongoDbMatteoFabbri.Services;
using System.Data;
using BlazorIdentityMongoDbMatteoFabbri.Shared;
using Microsoft.AspNetCore.Authorization;

namespace BlazorIdentityMongoDbMatteoFabbri
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            // If I include the code block below, the following error is
            // created during app.Run()
            // 
            // Scheme already exists: Identity.Application
            //builder.Services.AddAuthentication(options =>
            //    {
            //        options.DefaultScheme = IdentityConstants.ApplicationScheme;
            //        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //    })
            //    .AddIdentityCookies();

            MongoDbConfig? mongoDbConfig = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
            IdentityBuilder? builderResult = null;
            try
            {
                builderResult = builder.Services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(
                    identity =>
                    {
                        identity.Password.RequiredLength = 4;
                        identity.Password.RequireNonAlphanumeric = false;
                        identity.Password.RequireUppercase = false;
                        identity.Password.RequireLowercase = false;
                        identity.Password.RequireDigit = false;
                    },
                    mongo =>
                    {
                        mongo.ConnectionString = mongoDbConfig!.ConnectionString;
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // If MongoDB service is not running...
            if (builderResult == null)
            {
                Console.WriteLine("ERROR ERROR - MongoDB service may not be running!");
                Console.WriteLine("ERROR ERROR - MongoDB service may not be running!");
                Console.WriteLine("ERROR ERROR - MongoDB service may not be running!");
            }
            else
            {
                builder.Services
                    .AddIdentityCore<ApplicationUser>()
                    .AddRoles<ApplicationRole>()
                    .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(mongo =>
                    {
                        mongo.ConnectionString = mongoDbConfig!.ConnectionString;
                    })
                    .AddSignInManager();
                //.AddDefaultTokenProviders();  // Still confused about this method. When it's needed or not. Apparently, not, in my case.

                builder.Services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = IdentityConstants.ApplicationScheme;
                        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    })

                    .AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    });
                // .AddApplicationCookie();// Error when adding this line: "Scheme already exists: Identity.Application" (shooting in the dark!)
                // .AddIdentityCookies(); // Error when adding this line: "Scheme already exists: Identity.Application"

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy(Constants.ADMINPOLICYNAME,
                        policy => policy.RequireRole(Constants.ADMIN));

                    options.AddPolicy(Constants.SUPERADMINPOLICYNAME,
                        policy => policy.RequireRole(Constants.SUPERADMIN));

                    options.AddPolicy(Constants.SPECIALFLAGPOLICYNAME, policy => policy.Requirements.Add(new SpecialFlagRequirement()));

                });

            }  // end of else block          

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            builder.Services.AddScoped<IStudentService, StudentService>();

            builder.Services.AddScoped<IAuthorizationHandler, SpecialFlagHandler>();
            builder.Services.AddScoped<IAccessControlService, AccessControlService>();

            string? rootUserName = Environment.GetEnvironmentVariable(Constants.ENVVARSUPERUSERNAME);
            string? rootUserPassword = Environment.GetEnvironmentVariable(Constants.ENVVARSUPERUSERPASSWORD);

            if (rootUserName != null && rootUserPassword != null)
            {
                builder.Services.AddScoped<IRootInfoService, RootInfoService>(scooby => new(rootUserName)); // I don't understand how this syntax pulls in an expected (?) "IServiceProvider"
            }
            else
            {
                Console.WriteLine("ERROR !!: ENV VARIABLES COULD NOT BE RETRIEVED!");
                return;
            }

            var app = builder.Build();                           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            // seeding DB with super user account
            SeedingTheDatabase(builder, app, rootUserName, rootUserPassword);

            app.Run();
        }

        /// <summary>
        /// Seeds the database with a superuser role if the user does not exist.
        /// Expects the superuser name and password to be defined in ENV variables.
        /// </summary>
        private static void SeedingTheDatabase(WebApplicationBuilder builder, 
                                               WebApplication app,
                                               string rootUserName,
                                               string rootUserPassword)
        {
            using (var scope = app.Services.CreateScope())
            {
                // https://stackoverflow.com/questions/77904510/how-do-you-initialize-a-blazor-server-application-database-with-admin-user-on-ve

                var services = scope.ServiceProvider;
                UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                // just a test of my simple service - I gotta remember to use the Interace and not the implementation!
                IRootInfoService iRootInfoService = services.GetRequiredService<IRootInfoService>();
                string superUserName = iRootInfoService.GetRootUserName();

                // create admin roles
                AddRole(Constants.ADMIN, roleManager, app);
                AddRole(Constants.SUPERADMIN, roleManager, app);

                Task<ApplicationUser?> resultTask = userManager.FindByNameAsync(rootUserName);
                try
                {
                    if (resultTask.Result == null) // resultTask.Result is of type ApplicationUser. If null then User does not yet exist.
                    {
                        var user = Activator.CreateInstance<ApplicationUser>();
                        user.Email = "test@test.com";
                        user.UserName = rootUserName;

                        Task<IdentityResult> response = userManager.CreateAsync(user, rootUserPassword);

                        response.Wait();

                        if (!response.Result.Succeeded)
                        {
                            Console.WriteLine("ERROR SEEDING DATABASE with new USER!");
                            app.DisposeAsync();
                        }

                        response = userManager.AddToRoleAsync(user, Constants.SUPERADMIN); // add role to the user
                        response.Wait();
                        if (!response.Result.Succeeded)
                        {
                            Console.WriteLine("ERROR adding SuperAdmin role to seeded USER!");
                            app.DisposeAsync();
                        }

                        response = userManager.AddToRoleAsync(user, Constants.ADMIN); // add role to the user
                        response.Wait();
                        if (!response.Result.Succeeded)
                        {
                            Console.WriteLine("ERROR adding Admin role to seeded USER!");
                            app.DisposeAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR !!: " + ex.Message.ToString());
                    app.DisposeAsync();
                }
            }
        }
        private static void AddRole(string roleName, RoleManager<ApplicationRole> roleManager, WebApplication app)
        {
            Task<bool> roleAlreadyExists = roleManager!.RoleExistsAsync(roleName);
            roleAlreadyExists.Wait();

            if (roleAlreadyExists.IsCompletedSuccessfully && roleAlreadyExists.Result == false)
            {
                Task<IdentityResult> response2 = roleManager.CreateAsync(new ApplicationRole(roleName));
                response2.Wait();
                if (!response2.Result.Succeeded)
                {
                    Console.WriteLine($"ERROR CREATING {roleName} ROLE in DATABASE!");
                    app.DisposeAsync();
                }
            }
        }
    }

    static class Constants
    {
        public const string ADMIN = "Admin";        
        public const string ADMINPOLICYNAME = "RequireAdminRole";

        public const string SUPERADMIN = "SuperAdmin";
        public const string SUPERADMINPOLICYNAME = "RequireSuperAdminRole";

        public const string SPECIALFLAGPOLICYNAME = "SpecialFlagPolicy";

        public const string ENVVARSUPERUSERNAME = "SUPERUSERNAME";
        public const string ENVVARSUPERUSERPASSWORD = "SUPERUSERPASSWORD";
    }
}
