using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class BoADebitCsvDataProvider : ITransactionProvider
    {
        string[] lines;

        public BoADebitCsvDataProvider(string[] lines)
        {
            this.lines = lines;
        }

        public IEnumerable<string> GetBodyWithoutHeader()
        {
            return lines.Skip(8);
        }

        public IEnumerable<Transaction> GetTransactions()
        {                       
            if (!lines[0].Equals("Description,,Summary Amt.") ||
                !lines[6].Equals("Date,Description,Amount,Running Bal.") ||
                !lines[7].Contains("Beginning balance as of"))

                throw new Exception("Line 0 or 6 or 7 is not what I expect");
            
            var result = new List<Transaction>();
            var csvParser = new CsvLineParser();

            foreach (var line in GetBodyWithoutHeader())
            {                                                   
                var parsed = csvParser.Parse(line);
                
                var transaction = new Transaction();
                transaction.Date = DateTime.ParseExact(parsed[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                transaction.Description = parsed[1];                                
                transaction.Amount = double.Parse(parsed[2]);
                yield return transaction;
            }

            yield break;
        }
    }
}
