using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    //hago que a esta seccion pueda acceder usuarios autorizados
    [Authorize]
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;      
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SlidersController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
        }
        //---------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //---------------------------------------------------------------
        //voy al form de la vista create
        [HttpGet]
        public IActionResult Create()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            //valido
            if (ModelState.IsValid)
            {
                //obtengo ruta
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                //obtengo archivo
                var archivos = HttpContext.Request.Form.Files;
               
                    //articulo nuevo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName);

                //lo creo con su extension
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }
                    //creo carpeta slider en root.imagenes.sliders
                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension;                  


                    //lo agrego al contenedor
                    _contenedorTrabajo.Slider.Add(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                
            }
            return View();
        }

        //---------------------------------------------------------------
        //muestra el slider a editar que busca por el id
        [HttpGet]
        public IActionResult Edit(int? id)
        {          
            if (id != null)
            {
                var slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
                return View(slider);
            }

            //cuando el id es nulo y no lo encontro en la bbdd
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            //valido
            if (ModelState.IsValid)
            {
                //guardo ruta
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                //accedo a archivos
                var archivos = HttpContext.Request.Form.Files;
                //obtengo id
                var sliderDesdeDb = _contenedorTrabajo.Slider.Get(slider.Id);

                //si hay archivo imagen para subir
                if (archivos.Count() > 0)
                {
                    //articulo nuevo guid, ruta
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");                    
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    //borro la imagen anterior
                    var rutaImagen = Path.Combine(rutaPrincipal, sliderDesdeDb.UrlImagen.TrimStart('\\'));
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subo img nueva
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + nuevaExtension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + nuevaExtension;                   

                    //guardo en contenedor
                    _contenedorTrabajo.Slider.Update(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //si se conserva la misma imagen que estaba
                    slider.UrlImagen = sliderDesdeDb.UrlImagen;
                }

                //guardo los demas campos 
                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        
        #region LLAMADAS A LA API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Slider.GetAll() });
        }

        //---------------------------------------------------------------

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //busco slider por id
            var objFromDb = _contenedorTrabajo.Slider.Get(id);
            if (objFromDb == null)
            {
                //si es nulo envia a toastr este mensaje
                return Json(new { success = false, message = "Error borrando slider" });
            }
            //si no es nulo, se elimina y retorna otro json a toastr
            _contenedorTrabajo.Slider.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Slider borrado correctamente" });
        }
        #endregion
    }
}