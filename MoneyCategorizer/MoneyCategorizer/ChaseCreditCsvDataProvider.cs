using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class ChaseCreditCsvDataProvider : ITransactionProvider
    {
        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("Type,Trans Date,Post Date,Description,Amount");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent)
        {
            DataProviderExtensions.CheckFormatSupported(this, fileContent);
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent);
            var csvParser = new CsvLineParser();

            foreach (var line in lines.Skip(1))
            {
                var s = csvParser.Parse(line);

                var transaction = new Transaction();
                transaction.Date = DateTime.ParseExact(s[1], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                transaction.Description = s[3];
                transaction.Amount = double.Parse(s[4]);
                yield return transaction;
            }

            yield break;
        }
    }
}
