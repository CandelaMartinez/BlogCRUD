using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BlogCore.AccesoDatos.Data.Inicializador
{
    public class InicializadorDB : IInicializadorDB
    {
        //accedo al contexto
        private readonly ApplicationDbContext _db;

        //crear usuario administrador
        private readonly UserManager<ApplicationUser> _userManager;

        //usar los roles
        private readonly RoleManager<IdentityRole> _roleManager;

        public InicializadorDB(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public void Inicializar()
        {
            try
            {
                //accede a la bbdd
                //valido si hay migraciones pendientes
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    //si hay migraciones pendientes va a ejecutar la migracion
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                
            }
            //validacion para los roles: existe algun rol con el nombre admin? si es asi lo retorna
          
            if (_db.Roles.Any(ro => ro.Name == CNT.Admin)) return;           

            //si no existe: crea el role admin y crea el rol usuario
            _roleManager.CreateAsync(new IdentityRole(CNT.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Usuario)).GetAwaiter().GetResult();

            //creo un usuario por defecto administrador
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@candelamartinez.com",
                Email = "admin@candelamartinez.com",
                EmailConfirmed = true,
                Nombre = "Candela"
            }, "Admin1234#").GetAwaiter().GetResult();

            
            //accedo al contexto de la bbdd a la tabla applicationUser
            //obtengo de alli el usuario que tenga el email que le paso
            //le doy el rol al usuario
            ApplicationUser usuario = _db.ApplicationUser
                .Where(us => us.Email == "admin@candelamartinez.com")
                .FirstOrDefault();
            _userManager.AddToRoleAsync(usuario, CNT.Admin).GetAwaiter().GetResult();
        
        }
    }
}
