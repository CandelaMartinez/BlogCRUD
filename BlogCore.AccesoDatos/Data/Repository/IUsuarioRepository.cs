
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCore.AccesoDatos.Data.Repository
{
    //va al campo lockoutEnabled de la tabla aspnetUsers y cambia los valores segun esta bloqueado o no el usuario
    public interface IUsuarioRepository : IRepository<ApplicationUser>
    {
        void BloqueaUsuario(string IdUsuario);
        void DesbloquearUsuario(string IdUsuario);
    }
}
