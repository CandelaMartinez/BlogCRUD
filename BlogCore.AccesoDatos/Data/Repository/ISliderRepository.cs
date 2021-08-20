using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
//metodos propios de slider
namespace BlogCore.AccesoDatos.Data.Repository.IRepository
{
    public interface ISliderRepository : IRepository<Slider>
    {
        void Update(Slider slider);
    }
}
