using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract4.DAL.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        void Update(T entity);

        void Remove(T entity);

        IEnumerable<T> GetAll();

        T Get(int id);
    }
}
