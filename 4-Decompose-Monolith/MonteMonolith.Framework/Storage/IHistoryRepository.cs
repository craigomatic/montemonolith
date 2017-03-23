using MonteMonolith.Framework.Model;
using System.Collections.Generic;

namespace MonteMonolith.Framework.Storage
{
    public interface IHistoryRepository
    {
        void Create(MonteResult result);
        void Delete(MonteResult result);
        MonteResult Read(string id);
        void Update(MonteResult result);
        IEnumerable<MonteResult> FindAll();
    }
}