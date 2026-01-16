using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagementSystem.Interfaces;

namespace StudentManagementSystem.Data
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _data = new();
        private readonly Func<T, int> _idSelector;

        public InMemoryRepository(Func<T, int> idSelector)
        {
            _idSelector = idSelector;
        }

        public IEnumerable<T> GetAll() => _data;

        public T? GetById(int id)
        {
            return _data.FirstOrDefault(e => _idSelector(e) == id);
        }

        public void Add(T entity)
        {
            _data.Add(entity);
        }

        public void Update(T entity)
        {
            var existing = GetById(_idSelector(entity));
            if (existing != null)
            {
                var index = _data.IndexOf(existing);
                if (index != -1)
                {
                    _data[index] = entity;
                }
            }
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _data.Remove(existing);
            }
        }

        public IQueryable<T> Query()
        {
            return _data.AsQueryable();
        }
    }
}
