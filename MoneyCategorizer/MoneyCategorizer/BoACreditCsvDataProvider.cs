using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class BoACreditCsvDataProvider : ITransactionProvider
    {
        string[] lines;

        public BoACreditCsvDataProvider(string[] lines)
        {
            this.lines = lines;
        }

        public IEnumerable<string> GetBodyWithoutHeader()
        {
            return lines.Skip(1);
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            var result = new List<Transaction>();
            
            if (!lines[0].Equals("Posted Date,Reference Number,Payee,Address,Amount"))
                throw new Exception("First line doesn't look as expected to be");

            var csvParser = new CsvLineParser();

            foreach (var line in GetBodyWithoutHeader())
            {
                var s = csvParser.Parse(line);
                
                var transaction = new Transaction();
                transaction.Date = DateTime.ParseExact(s[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);                
                transaction.Description = s[2];                
                transaction.Amount = double.Parse(s[4]);
                yield return transaction;
            }

            yield break;
        }
    }
}
