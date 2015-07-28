using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class CategorizedTransaction
    {
        public string Category { get; set; }
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        public double Amount
        {
            get { return Transactions.Sum(x => x.Amount); }
        }
    }
}
