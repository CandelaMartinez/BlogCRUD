using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlogCore.AccesoDatos.Data;
using BlogCore.AccesoDatos.Data.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using BlogCore.Utilidades;
using BlogCore.Models;
using BlogCore.AccesoDatos.Data.Inicializador;

namespace BlogCore
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //IDENTITY: cambio aqui a add.identity para que no deje el usuario por defecto sino el usuario que le paso

            //ojo: cambie indetityUser por BlogCore.Models.Applicationuser
            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();



            //IDENTITY: agrego nuevo service
            services.AddSingleton<IEmailSender, EmailSender>();

            //agrego el contenedor de trabajo
            services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();


            //agrego el servicio que hace el llamado para consumir la clase inicializadora y su interface
            services.AddScoped<IInicializadorDB, InicializadorDB>();

            //agregue razor runtime compilation
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
           


            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //agrego una variable de la interface IInicializadorDB como parametro del metodo Configure
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInicializadorDB dbInicial)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //llamo al metodo inicializar() usando la variable de clase que pase en el constructor
            dbInicial.Inicializar();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Cliente}/{controller=Home}/{action=Index}/{id?}");//la aplicacion inicia por cliente
                endpoints.MapRazorPages();
            });
        }
    }
}
