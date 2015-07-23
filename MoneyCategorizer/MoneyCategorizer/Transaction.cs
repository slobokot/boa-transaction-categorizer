using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    [System.Diagnostics.DebuggerDisplay("{Description} = {Amount}")]
    class Transaction
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public double Amount { get; set; }
    }
}
