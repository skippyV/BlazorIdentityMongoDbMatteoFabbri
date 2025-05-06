using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo;
using BlazorIdentityMongoDbMatteoFabbri.Components;
using BlazorIdentityMongoDbMatteoFabbri.Components.Account;
using BlazorIdentityMongoDbMatteoFabbri.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using BlazorIdentityMongoDbMatteoFabbri.Services;

namespace BlazorIdentityMongoDbMatteoFabbri
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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
            IdentityBuilder builderResult = null;
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

                // I DO NOT THINK I can just add AddAuthentication() here because
                // this was working previously without using .AddAuthentication()
                // ....but then how to add AddGoogle() ?

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
            }

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            builder.Services.AddScoped<IStudentService, StudentService>();

            string? rootUserName = Environment.GetEnvironmentVariable("SUPERUSERNAME");
            string? rootUserPassword = Environment.GetEnvironmentVariable("SUPERUSERPASSWORD");

            if (rootUserName != null && rootUserPassword != null)
            {
                builder.Services.AddScoped<IRootInfoService, RootInfoService>(serviceProvider => new(rootUserName)); // I don't understand how this syntax pulls in an expected (?) "IServiceProvider"
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
            using (var scope = app.Services.CreateScope())
            {
                // https://stackoverflow.com/questions/77904510/how-do-you-initialize-a-blazor-server-application-database-with-admin-user-on-ve

                var services = scope.ServiceProvider;
                UserManager<ApplicationUser> mgr = services.GetRequiredService<UserManager<ApplicationUser>>();

                // just a test of my simple service - I gotta remember to use the Interace and not the implementation!
                IRootInfoService iRootInfoService = services.GetRequiredService<IRootInfoService>();
                string superUserName = iRootInfoService.GetRootUserName();

                Task<ApplicationUser?> resultTask = mgr.FindByNameAsync(rootUserName);
                try
                {
                    if (resultTask.Result == null) // resultTask.Result is of type ApplicationUser
                    {
                        var user = Activator.CreateInstance<ApplicationUser>();
                        user.Email = "test@test.com";
                        user.UserName = rootUserName;

                        Task<IdentityResult> identityResult = mgr.CreateAsync(user, rootUserPassword);

                        identityResult.Wait();

                        if (!identityResult.Result.Succeeded)
                        {
                            Console.WriteLine("ERROR SEEDING DATABASE!");
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

            app.Run();
        }
    }
}
