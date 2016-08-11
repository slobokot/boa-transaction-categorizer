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
        readonly string transactionStart = "Date,Description,Amount,Running Bal.";

        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("Description,,Summary Amt.") &&
                fileContent.Contains(transactionStart);
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent)
        {
            DataProviderExtensions.CheckFormatSupported(this, fileContent);
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent.Substring(fileContent.IndexOf(transactionStart)));
            var csvParser = new CsvLineParser();

            foreach (var line in lines.Skip(1))
            {
                var parsed = csvParser.Parse(line);

                if (parsed[1].StartsWith("Beginning balance as of "))
                {
                    continue;
                }

                var transaction = new Transaction
                {
                    Date = DateTime.ParseExact(parsed[0], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                    Description = parsed[1],
                    Amount = double.Parse(parsed[2])
                };
                yield return transaction;
            }

            yield break;
        }
    }
}
