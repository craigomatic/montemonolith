using Microsoft.AspNetCore.Mvc;
using MonteMonolith.Framework;
using MonteMonolith.Framework.Model;
using MonteMonolith.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MonteMonolith.Apis
{
    [Route("api/[controller]")]
    public class SimulationController : Controller
    {
        public IHistoryRepository HistoryRepository { get; private set; }

        public SimulationController(IHistoryRepository historyRepository)
        {
            this.HistoryRepository = historyRepository;
        }

        [HttpGet("simulate/{total}/{tMin}/{tMod}/{tMax}")]
        public double[] Simulate(int total, string tMin, string tMod, string tMax)
        {
            var tMinNum = _StringToArray(tMin);
            var tMaxNum = _StringToArray(tMax);
            var tModNum = _StringToArray(tMod);

            var result = MonteCarlo.simulate(total, tMinNum, tModNum, tMaxNum);

            var monteResult = new MonteResult
            {
                Id = Guid.NewGuid().ToString(),
                InputMax = tMaxNum,
                InputMin = tMinNum,
                InputMod = tModNum,
                InputTotal = total,
                Result = result
            };

            this.HistoryRepository.Create(monteResult);

            return result;
        }
        
        private static double[] _StringToArray(string tMin)
        {
            var split = tMin.Split(',');

            if (split.Length == 0)
            {
                return new[] { double.Parse(tMin) };
            }

            var toReturn = new List<double>();

            foreach (var item in split)
            {
                toReturn.Add(double.Parse(item));
            }

            return toReturn.ToArray();
        }
    }
}
