using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Chase2020CreditCsvDataProvider : ITransactionProvider
    {
        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("Type,Trans Date,Post Date,Description,Amount");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent, string fileName)
        {
            DataProviderExtensions.CheckFormatSupported(this, fileContent);
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent);
            var csvParser = new CsvLineParser();

            foreach (var line in lines.Skip(1))
            {
                var transaction = new Transaction();
                try
                {
                    var s = csvParser.Parse(line);
                    
                    transaction.Date = DateTime.ParseExact(s[1], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    transaction.Description = s[3];
                    transaction.Amount = double.Parse(s[4]);
                    transaction.Raw = line;
                    transaction.FileName = fileName;
                }
                catch 
                {
                    Console.WriteLine($"Failed for line {line} in file '{fileName}'");
                    throw;
                }
                yield return transaction;
            }

            yield break;
        }
    }
}
