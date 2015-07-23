using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class TransactionProviderFactory
    {
        public ITransactionProvider GetTransactionProvider(string[] lines)
        {            
            if (lines[0].StartsWith("Description"))
                return new BoADebitCsvDataProvider(lines);

            return new BoACreditCsvDataProvider(lines);
        }
    }
}
