using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    //hago que a esta seccion pueda acceder usuarios autorizados
    [Authorize]

    //mapeo a que area pertenece
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        //---------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //---------------------------------------------------------------
        //muestra vista Create (formulario vacio que valida campos que ingreso)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //metodo que se activa con el boton crear, de la vista parcial CrearVolver, llamada desde Create.cshtml
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            //valida el modelo
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Categoria.Add(categoria);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        //---------------------------------------------------------------
        //recibe el id desde categorias.edit.cshtml y busca la entidad categoria por su id
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Categoria categoria = new Categoria();
            categoria = _contenedorTrabajo.Categoria.Get(id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        //recibe el modelo categoria, lo valida, lo guarda al contenedor de trabajo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Categoria.Update(categoria);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }


        #region LLAMADAS A LA API
        //es para obtener
        [HttpGet]
        public IActionResult GetAll()
        {
            //devuelve en formato Json, data tendra todo el repositorio de categoria
            return Json(new { data = _contenedorTrabajo.Categoria.GetAll() });
        }

        //---------------------------------------------------------------
        //va dentro del llamado a api porque esta dentro de categorias.js el metodo delete
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //busca la categoria
            var objFromDb = _contenedorTrabajo.Categoria.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando categoria" });
            }

            //la borra
            _contenedorTrabajo.Categoria.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Categoría borrada correctamente" });
        }
        #endregion
    }
}