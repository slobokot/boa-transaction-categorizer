using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var directory = args.Length > 0 ? args[0] : ".";

                var transactions = ReadAllTransactionsFromDirectory(directory);
                var periods = GetPeriods();
                periods.ForEach(period => Run(transactions, period));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        static List<Period> GetPeriods()
        {
            var periods = new List<Period>();
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            for (int i = 0; i < 6; i++)
            {
                DateTime from = new DateTime(now.Year, now.Month, 1);
                DateTime to = from.AddMonths(1);
                periods.Add(new Period { From = from, To = to });
                now = now.AddMonths(-1);
            }

            periods.Add(new Period { From = new DateTime(DateTime.Now.Year, 1, 1), To = new DateTime(DateTime.Now.Year + 1, 1, 1) });
            if (DateTime.Now.Month <= 2)
            {
                periods.Add(new Period { From = new DateTime(DateTime.Now.Year - 1, 1, 1), To = new DateTime(DateTime.Now.Year, 1, 1) });
            }

            return periods;
        }

        static IEnumerable<Transaction> ReadAllTransactionsFromDirectory(string directory)
        {
            var dataProviders = new ITransactionProvider[] {
                new RbcCsvDataProvider(),
                new BoACreditCsvDataProvider(),
                new BoADebitCsvDataProvider(),
                new ChaseCreditCsvDataProvider(),
                new Chase2018CreditCsvDataProvider()
            };

            var transactions = new List<Transaction>();
            var fileUniqueness = new FileContentUniqueness();

            foreach (var file in Directory.EnumerateFiles(directory, "*.csv"))
            {
                try
                {
                    var fileContent = File.ReadAllText(file);

                    var dataProvider = (from x in dataProviders where x.FormatSupported(fileContent) select x).ToList();
                    if (dataProvider.Count != 1)
                    {
                        throw new Exception($"{file} format is not supported");
                    }
                    transactions.AddRange(dataProvider[0].GetTransactions(fileContent));
                }
                catch
                {
                    Console.WriteLine($"Failed for file {file}");
                    throw;
                }
            }

            fileUniqueness.AssertUnique(transactions);

            return transactions;
        }

        static void Run(IEnumerable<Transaction> transactions, Period period)
        {
            var filteredTransactions = Filter(transactions, period);

            var categorizer = new Categorizer();
            var categorized = categorizer.Categorize(filteredTransactions);

            Reporter.Report(categorized, period);
        }

        static IEnumerable<Transaction> Filter(IEnumerable<Transaction> transactions, Period period)
        {
            return from t in transactions
                   where (period.From <= t.Date && t.Date < period.To)
                   select t;
        }
    }
}
