using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonteMonolith.Framework.Model;

namespace MonteMonolith.Framework.Storage
{
    public interface IContext<T>
    {
        IEnumerable<T> Entities { get; }

        T Read(string id);
        void Update(T result);
        void Delete(T result);
        void Create(T result);
    }
}
