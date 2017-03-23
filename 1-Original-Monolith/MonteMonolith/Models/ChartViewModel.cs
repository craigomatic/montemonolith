using MonteMonolith.Framework.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonteMonolith.Models
{
    public class ChartViewModel
    {
        public IEnumerable<MonteResult> Dataset { get; private set; }

        public ChartViewModel(IEnumerable<MonteResult> dataset)
        {
            this.Dataset = dataset;
        }

        public IHtmlString Labels
        {
            get
            {
                //just generate empty strings for each item in the dataset
                var labels = new List<string>();

                for (int i = 0; i < this.Dataset.Count(); i++)
                {
                    labels.Add(string.Empty);
                }

                return new MvcHtmlString(JsonConvert.SerializeObject(labels));
            }
        }

        public IHtmlString Totals
        {
            get
            {
                //just generate empty strings for each item in the dataset
                var totals = new List<double>();

                foreach (var item in this.Dataset)
                {
                    //note that for simplicity we're just taking the first result
                    totals.Add(item.Result.First());
                }

                return new MvcHtmlString(JsonConvert.SerializeObject(totals));
            }
        }
    }
}