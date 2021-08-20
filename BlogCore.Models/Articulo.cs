using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

//tabla articulo con data anottation con sus campos
namespace BlogCore.Models
{
    public class Articulo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del artículo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El descripción es obligatoria")]       
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de creación")]
        public string FechaCreacion { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        public string UrlImagen { get; set; }

        //relacion: 1 categoria muchos articulos
        [Required]
        public int CategoriaId { get; set; }

        //referencio a la tabla categoria
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

    }
}
