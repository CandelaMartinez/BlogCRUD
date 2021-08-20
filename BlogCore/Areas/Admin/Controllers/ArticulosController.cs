using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    //hago que a esta seccion pueda acceder usuarios autorizados
    [Authorize]
    //añadir area
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        //acceso al contenedor
        private readonly IContenedorTrabajo _contenedorTrabajo;
        //para permitir subida de archivos (img)
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironmen)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironmen;
        }
        //-------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            

            return View();
        }
        //-------------------------------------------------------------------------------
        //muestra el formulario, trabaja sobre el viewModel asi veo los campos de las tablas relacionadas
        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM artivm = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(artivm);
        }

        //metodo que guarda la informacion recibida del form Create en la bbdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM artiVM)
        {
            //valido modelo 
            if (ModelState.IsValid)
            {
                //accedo a ruta principal: www.root
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                //
                var archivos = HttpContext.Request.Form.Files;

                if (artiVM.Articulo.Id == 0)
                {
                    //Nuevo articulo
                    //guid para crear un nuevo nombre para el archivo img
                    string nombreArchivo = Guid.NewGuid().ToString();
                    //creo la carpeta root.imagenes.articulos 
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
                    //obtengo la extension del archivo
                    var extension = Path.GetExtension(archivos[0].FileName);

                    //filestreams recibe la ruta completa, y crea nuevo archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {

                        archivos[0].CopyTo(fileStreams);
                    }

                    //completo los campos de articulo
                    artiVM.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension;
                    //la fecha de creacion se genera automaticamente
                    artiVM.Articulo.FechaCreacion = DateTime.Now.ToString();

                    //guardo en el contenedor de trabajo
                    _contenedorTrabajo.Articulo.Add(artiVM.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
            }

            //poblo la lista de categorias en el caso que no se cumpla el if
            artiVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            return View(artiVM);
        }

        //-------------------------------------------------------------------------------
        //busca el articulo por el id y me lo muestra para que lo pueda editar
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            //instancio viewmodel
            ArticuloVM artivm = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            //valido
            if (id != null)
            {
                artivm.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
            }
            return View(artivm);
        }

        //guarda el articulovm recibido del form en la bbdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM artiVM)
        {
            //valido
            if (ModelState.IsValid)
            {
                //obtengo ruta
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                //accedo a los archivos
                var archivos = HttpContext.Request.Form.Files;

                //obtengo por id el articulo que quiero editar
                var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(artiVM.Articulo.Id);

                //si si se subio una imagen por elcampo de subir imagen
                if (archivos.Count() > 0)
                {
                    //Edito imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);
                    
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    //nueva ruta de imagen que reemplaza la anterior
                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));

                    //si la imagen existe en esa ruta, se borra porque va a ser reemplazada, asi no guardo img viejas
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subo el archivo nuevo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + nuevaExtension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    artiVM.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + nuevaExtension;
                    artiVM.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _contenedorTrabajo.Articulo.Update(artiVM.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                   //si la imagen ya existe no se reemplaza, debe conservar la que ya esta enla bbdd
                    artiVM.Articulo.UrlImagen = articuloDesdeDb.UrlImagen;
                }

                //guardo todos los campos actualizados en el contenedor
                _contenedorTrabajo.Articulo.Update(artiVM.Articulo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //-------------------------------------------------------------------------------
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //obtengo el articulo a eliminar por el id
            var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(id);
            //obtengo la ruta donde esta guardada la img
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            //elimino los caracteres \ de la ruta
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));

            //validacion: si existe, lo borra del directorio
            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }
            //si es nulo
            if (articuloDesdeDb == null)
            {
                return Json(new { success = false, message = "Error borrando artículo"});
            }

            //lo borro del contenedor
            _contenedorTrabajo.Articulo.Remove(articuloDesdeDb);
            _contenedorTrabajo.Save();
            //retorno json 
            return Json(new { success = true, message = "Artículo borrado con éxito" });

        }

        //---------------------------------------------------------------------------------------------------
        //incluyo categorias asi puedo acceder a la tabla relacionada
        #region LLAMADAS A LA API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }        
        #endregion

    }
}