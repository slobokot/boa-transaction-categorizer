using System;

namespace MoneyCategorizer
{
    [System.Diagnostics.DebuggerDisplay("{Description} = {Amount}")]
    public class Transaction
    {
        private static int idCounter = 0;
        private int id;
        private string raw;
        
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string FileName { get; set; }
        public string Raw
        {
            get { return raw; }
            set { raw = value.Trim(); }
        }

        public int Id
        {
            get { return id; }
        }

        public Transaction()
        {
            id = idCounter++;
        }
    }
}
