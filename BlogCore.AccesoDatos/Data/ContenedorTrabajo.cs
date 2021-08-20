using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCore.AccesoDatos.Data
{
    //el contenedor tiene dentro todos los repositorios
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;
        public ICategoriaRepository Categoria { get; private set; }
        public IArticuloRepository Articulo { get; private set; }
        public ISliderRepository Slider { get; private set; }
        public IUsuarioRepository Usuario { get; private set; }

        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Categoria = new CategoriaRepository(_db);
            Articulo = new ArticuloRepository(_db);
            Slider = new SliderRepository(_db);
            Usuario = new UsuarioRepository(_db);
        }

     

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
