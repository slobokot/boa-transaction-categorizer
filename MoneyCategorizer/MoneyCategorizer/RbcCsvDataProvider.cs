using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class RbcCsvDataProvider : ITransactionProvider
    {
        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("\"Account Type\",\"Account Number\",\"Transaction Date\",\"Cheque Number\",\"Description 1\",\"Description 2\",\"CAD$\",\"USD$\"");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent)
        {
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent);
            var csvParser = new CsvLineParser();            

            foreach (var line in lines.Skip(1))
            {                
                var parsed = csvParser.Parse(line);

                var transaction = new Transaction();
                try
                {
                    transaction.Date = DateTime.ParseExact(parsed[2], "M/d/yyyy", CultureInfo.InvariantCulture);
                    transaction.Description = $"{parsed[4]}. {parsed[5]}";
                    transaction.Amount = double.Parse(parsed[6]);
                    if (!string.IsNullOrWhiteSpace(parsed[7]))
                        throw new NotImplementedException($"USD value in {line}");
                    transaction.Raw = line;
                }
                catch
                {
                    Console.WriteLine($"Failed for {line}");
                    throw;
                }                
                yield return transaction;
            }

            yield break;
        }
    }
}
