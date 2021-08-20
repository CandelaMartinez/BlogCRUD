using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCore.AccesoDatos.Data.Repository
{
    //metodos propios de articulo que hereda los metodos comunes de Irepository
    public interface IArticuloRepository : IRepository<Articulo>
    {      
        void Update(Articulo articulo);
    }
}
