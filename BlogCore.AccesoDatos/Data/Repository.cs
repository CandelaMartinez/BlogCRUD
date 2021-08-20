using BlogCore.AccesoDatos.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BlogCore.AccesoDatos.Data
{
    //implementa la interface IRepository
    public class Repository<T> : IRepository<T> where T : class
    {
        //defino contexto
        protected readonly DbContext Context;
        internal DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            Context = context;
            this.dbSet = context.Set<T>();

        }

        //añade entidad a la bbdd
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        //busca por id dentro de mi bbdd
        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        //devuelve todos 
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            //filtro
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            //include properties: por si quiero añadir otros parametros relacionados
            if (includeProperties != null)
            {
                //separo las propiedades que estan separadas por , y borre las vacias
                foreach (var includeProperty in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    //incluye las propiedades que le pase
                    query = query.Include(includeProperty);
                }
            }

            //ordenamiento
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }


            return query.ToList();
        }

        //devuelve un registro que cumple las expresiones de busqueda
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            //include properties
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entityToRemove = dbSet.Find(id);
            Remove(entityToRemove);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
