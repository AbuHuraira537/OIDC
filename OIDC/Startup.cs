
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OIDC.Models;
using OIDC.Profiles;

namespace OIDC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(Startup).Assembly.GetName().Name;
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString),op=>op.MigrationsAssembly(assembly));
                
            });
            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
               .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
           

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(config =>
                {
                    config.ConfigureDbContext = context => context.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),op=>op.MigrationsAssembly(assembly));
                    
                }).AddOperationalStore(store =>
                {
                    store.ConfigureDbContext = context => context.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), op => op.MigrationsAssembly(assembly));
                })
                //.AddInMemoryApiResources(Configurations.GetApis())
                //.AddInMemoryApiScopes(Configurations.GetApiScopes())
                //.AddInMemoryClients(Configurations.GetClients())
                //.AddInMemoryIdentityResources(Configurations.GetIdentityResources())
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();
            

            

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
