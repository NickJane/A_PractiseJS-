using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication2.Repository
{
    public interface IRepository<T> where T:class
    {
        void Insert(T entity);
        IEnumerable<T> GetSysUsers();
    }


    public class EfRepository<T> : IRepository<T> where T : class {
        private IDbSet<T> _entities;
        private WebApplication2.DAL.XEngineContext context;

        public EfRepository(WebApplication2.DAL.XEngineContext _context) { 
            context=_context;
            _entities = _context.Set<T>();
        }

        public void Insert(T entity) { }
        public IEnumerable<T> GetSysUsers() {
            return _entities;
        }
    }
}