using System;
using System.Collections.Generic;
using System.Text;
using BlogCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.AccesoDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //tablas
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Slider> Slider { get; set; }

        //tabla identity
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
