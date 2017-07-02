using System;

namespace MoneyCategorizer
{
    [System.Diagnostics.DebuggerDisplay("{Description} = {Amount}")]
    class Transaction
    {
        private string raw;

        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Raw
        {
            get { return raw; }
            set { raw = value.Trim(); }
        }
    }
}
