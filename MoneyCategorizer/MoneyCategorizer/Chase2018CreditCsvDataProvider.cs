﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Chase2018CreditCsvDataProvider : ITransactionProvider
    {
        public bool FormatSupported(string fileContent)
        {
            return fileContent.StartsWith("Details,Posting Date,Description,Amount,Type,Balance,Check or Slip #");
        }

        public IEnumerable<Transaction> GetTransactions(string fileContent)
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
                    transaction.Description = s[2];
                    transaction.Amount = double.Parse(s[3]);
                    transaction.Raw = line;                    
                }
                catch 
                {
                    Console.WriteLine($"Failed for line {line}");
                    throw;
                }
                yield return transaction;
            }

            yield break;
        }
    }
}
