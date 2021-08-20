using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogCore.AccesoDatos.Data
{
    public class UsuarioRepository : Repository<ApplicationUser>, IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void BloqueaUsuario(string IdUsuario)
        {
            //obtener el usuario por el usuario
            var usarioDesdeDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == IdUsuario);
            //llamo al metodo LockoutEnd ahora por 100 años
            usarioDesdeDb.LockoutEnd = DateTime.Now.AddYears(100);
            _db.SaveChanges();
        }

        public void DesbloquearUsuario(string IdUsuario)
        {
            //obtener el usuario por el usuario
            var usarioDesdeDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == IdUsuario);
            //llamo al metodo LockoutEnd now
            usarioDesdeDb.LockoutEnd = DateTime.Now;
            _db.SaveChanges();
        }
    }
}
