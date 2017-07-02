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
        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("Posted Date,Reference Number,Payee,Address,Amount");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent)
        {
            DataProviderExtensions.CheckFormatSupported(this, fileContent);
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent);            
            var csvParser = new CsvLineParser();

            foreach (var line in lines.Skip(1))
            {
                var s = csvParser.Parse(line);

                var transaction = new Transaction {
                    Date = DateTime.ParseExact(s[0], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                    Description = s[2],
                    Amount = double.Parse(s[4]),
                    Raw = line};

                yield return transaction;
            }

            yield break;
        }
    }
}
