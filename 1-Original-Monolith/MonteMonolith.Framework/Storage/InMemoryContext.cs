using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonteMonolith.Framework.Model;

namespace MonteMonolith.Framework.Storage
{
    public class InMemoryContext<T> : IContext<T> where T : IEntity
    {
        private List<T> _Data;

        public InMemoryContext()
        {
            _Data = new List<T>();
        }

        public IEnumerable<T> Entities
        {
            get { return _Data; }
        }

        public void Create(T result)
        {
            _Data.Add(result);   
        }

        public void Delete(T result)
        {
            _Data.Remove(result);
        }

        public T Read(string id)
        {
            return _Data.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Update(T result)
        {
            //not needed, object is in memory
        }
    }
}
