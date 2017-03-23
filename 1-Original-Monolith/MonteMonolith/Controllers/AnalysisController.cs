using MonteMonolith.Framework.Storage;
using MonteMonolith.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Generic;
using MonteMonolith.Framework.Model;

namespace MonteMonolith.Controllers
{
    public class AnalysisController : Controller
    {
        public IHistoryRepository HistoryRepository { get; private set; }

        public AnalysisController(IHistoryRepository historyRepository)
        {
            this.HistoryRepository = historyRepository;
        }

        public ActionResult Index()
        {
            var results = this.HistoryRepository.FindAll();

            //TODO: remove this
            var random = new Random();

            for (int i = 0; i < 55; i++)
            {
                (results as List<MonteResult>).Add(new MonteResult { Result = new[] { random.NextDouble() * 5d } });
            }

            return View(new ChartViewModel(results));
        }
    }
}
