using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    public class IdentityMap<T> where T : Entity
    {
        private Dictionary<int, T> _entities = new Dictionary<int, T>();

        public T Get(int id)
        {
            if (_entities.ContainsKey(id)) {  return _entities[id]; }

            return default(T);
        }

        public void Add(T entity)
        {
            _entities.Add(entity.Id, entity);
        }
    }
}
