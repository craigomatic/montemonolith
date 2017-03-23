using MonteMonolith.Framework.Storage;
using MonteMonolith.Models;
using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using MonteMonolith.Framework.Model;
using Microsoft.AspNetCore.Mvc;

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

            return View(new ChartViewModel(results));
        }
    }
}
