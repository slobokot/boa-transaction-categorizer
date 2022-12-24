using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    [System.Diagnostics.DebuggerDisplay("{Description} = {Amount}")]
    public class SortedTransaction
    {        
        private string raw;

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
    }
}
