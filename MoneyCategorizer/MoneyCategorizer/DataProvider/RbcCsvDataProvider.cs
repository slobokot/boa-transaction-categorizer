using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class RbcCsvDataProvider : ITransactionProvider
    {
        double? usdToCadRate = null;

        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("\"Account Type\",\"Account Number\",\"Transaction Date\",\"Cheque Number\",\"Description 1\",\"Description 2\",\"CAD$\",\"USD$\"");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent, string fileName)
        {
            var lines = DataProviderExtensions.SplitStringIntoLines(fileContent);
            var csvParser = new CsvLineParser();            

            foreach (var line in lines.Skip(1))
            {                
                var parsed = csvParser.Parse(line);

                var transaction = new Transaction();
                try
                {
                    transaction.FileName = fileName;
                    transaction.Date = DateTime.ParseExact(parsed[2], "M/d/yyyy", CultureInfo.InvariantCulture);
                    transaction.Description = $"{parsed[4]}. {parsed[5]}";
                    var cadAmound = parsed[6];
                    var usdAmound = parsed[7];
                    if (string.IsNullOrWhiteSpace(cadAmound))
                    {
                        if (string.IsNullOrWhiteSpace(usdAmound))
                        {
                            throw new Exception("both cad and usd amount are empty");
                        }
                        transaction.Amount = double.Parse(usdAmound) * UsdToCadRate;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(usdAmound))
                        {
                            throw new Exception("both cad and usd amount are set");
                        }
                        transaction.Amount = double.Parse(cadAmound);
                    }
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

        double UsdToCadRate
        {
            get
            {
                if (!usdToCadRate.HasValue)
                {
                    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "UsdToCadRate.txt").Replace("file:\\", "");
                    if (!File.Exists(path))
                    {
                        throw new Exception($"{path} file does not exist");
                    }
                    usdToCadRate = double.Parse(File.ReadAllLines(path)[0].Trim());
                }

                return usdToCadRate.Value;
            }
        }
    }
}
