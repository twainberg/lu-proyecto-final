using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LU.Logging;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDataContext>(options =>
                options.UseSqlServer(Program.Configuration["ConnectionStrings:AuthConnection"]));

            services.AddMvc();
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 6;
                        options.Password.RequiredUniqueChars = 1;
                        options.Password.RequireLowercase = false; 
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
                    .AddEntityFrameworkStores<ApplicationDataContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Login/AccesoDenegado");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}");
            });

            CrearRoles(serviceProvider).Wait();
        }

        private async Task CrearRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roles = { "Admin", "Alumno" };
            IdentityResult ResultadoRol;

            foreach (var rol in roles)
            {
                var ExisteRol = await RoleManager.RoleExistsAsync(rol);
                if (!ExisteRol)
                {
                    ResultadoRol = await RoleManager.CreateAsync(new IdentityRole<Guid>(rol));
                }
            }

            var Administrador = new ApplicationUser
            {
                UserName = Configuration["Administrador:Usuario"]
            };

            string AdministradorContraseña = Configuration["Administrador:Contraseña"];
            var _usuario = await UserManager.FindByNameAsync(Configuration["Administrador:Usuario"]);

            if(_usuario == null)
            {
                var crearAdministrador = await UserManager.CreateAsync(Administrador, AdministradorContraseña);
                if (crearAdministrador.Succeeded)
                {
                    await UserManager.AddToRoleAsync(Administrador, "Admin");
                }
            }
        }
    }
}
