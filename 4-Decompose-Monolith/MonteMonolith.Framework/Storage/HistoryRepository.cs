using MonteMonolith.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteMonolith.Framework.Storage
{
    public class HistoryRepository : IHistoryRepository
    {
        public IContext<MonteResult> Context { get; private set; }

        public HistoryRepository(IContext<MonteResult> context)
        {
            this.Context = context;
        }

        public void Create(MonteResult result)
        {
            this.Context.Create(result);
        }

        public MonteResult Read(string id)
        {
            return this.Context.Read(id);
        }
        
        public void Update(MonteResult result)
        {
            this.Context.Update(result);
        }

        public void Delete(MonteResult result)
        {
            this.Context.Delete(result);
        }

        public IEnumerable<MonteResult> FindAll()
        {
            return this.Context.Entities;
        }
    }
}
