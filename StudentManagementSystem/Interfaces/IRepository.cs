using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagementSystem.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        IQueryable<T> Query();
    }
}
