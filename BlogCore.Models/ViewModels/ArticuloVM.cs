using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCore.Models.ViewModels
{
    public class ArticuloVM
    {
        //instancio articulo y lista de categorias
        public Articulo Articulo { get; set; }
        public IEnumerable<SelectListItem> ListaCategorias { get; set; }
    }
}
