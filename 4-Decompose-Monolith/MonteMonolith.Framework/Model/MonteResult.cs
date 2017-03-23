using MonteMonolith.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteMonolith.Framework.Model
{
    public class MonteResult : IEntity
    {
        public string Id { get; set; }

        public double[] Result { get; set; }

        public int InputTotal { get; set; }

        public double[] InputMin { get; set; }

        public double[] InputMax { get; set; }

        public double[] InputMod { get; set; }
    }
}
