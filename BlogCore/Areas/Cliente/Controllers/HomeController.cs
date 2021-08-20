using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogCore.Models;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models.ViewModels;

namespace BlogCore.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        //instancio el contenedor de trabajo donde tengo todos los repositorios
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HomeController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            //viewmodel para usar articulos y slider
            HomeVM homeVm = new HomeVM()
            {
                Slider = _contenedorTrabajo.Slider.GetAll(),
                ListaArticulos = _contenedorTrabajo.Articulo.GetAll()
            };
            return View(homeVm);
        }

        public IActionResult Details(int id)
        {
            //traigo el articulo segun el id que me paso la vista index
            var articuloDesdeDb = _contenedorTrabajo.Articulo.GetFirstOrDefault(a => a.Id == id);
           //retorno la vista de details
            return View(articuloDesdeDb);
        }
       
    }
}
