using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogCore.Models
{
    public class Categoria
    {
        //dataAnnotation: clave primaria
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingresa un nombre para la categoría")]
        [Display(Name ="Nombre Categoria")]
        public string Nombre { get; set; }

        //dataAnotation: nombe de visualizacion
        [Required]
        [Display(Name = "Orden de visualizacion")]
        public int Orden { get; set; }
    }
}
