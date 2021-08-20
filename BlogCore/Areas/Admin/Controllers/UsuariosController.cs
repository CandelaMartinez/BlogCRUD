using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    //hago que a esta seccion pueda acceder usuarios autorizados
    [Authorize]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        //accedo al contenedor
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public UsuariosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        public IActionResult Index()
        {
            //llamo al usuario logueado
            //identity: accedo al usuario auntenticado y lo guardo en la var
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            //identity: busca x id del usuario
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //accedo a la lista de usuarios con todos los usuarios excepto mi usuario
            return View(_contenedorTrabajo.Usuario.GetAll(u => u.Id != usuarioActual.Value));
        }

        //recibe el id desde el form
        public IActionResult Bloquear(string id)
        {
            //valido
            if (id == null)
            {
                return NotFound();
            }

            //llamo al metodo bloquear usuario que esta en el repositorio
            _contenedorTrabajo.Usuario.BloqueaUsuario(id);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Desbloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _contenedorTrabajo.Usuario.DesbloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }

    }
}