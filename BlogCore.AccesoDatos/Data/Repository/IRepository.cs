using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//metodos comunes a todas las interfaces del repositorio
namespace BlogCore.AccesoDatos.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        IEnumerable<T> GetAll(

            //en caso de querer filtrar, por defecto null a menos que le pase ese parametro
            Expression<Func<T, bool>> filter = null,

            //en caso de querer ordenar, por defecto null a menos que le pase ese parametro
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,

            //cuando quiero incluir propiedades de otra tabla
            string includeProperties = null
         );

        T GetFirstOrDefault(
            //expresion, propiedades de otra tabla
          Expression<Func<T, bool>> filter = null,
           string includeProperties = null
        );

        void Add(T entity);
        
        //remover por id
        void Remove(int id);
        
        //remover por entidad
        void Remove(T entity);
    }
}
