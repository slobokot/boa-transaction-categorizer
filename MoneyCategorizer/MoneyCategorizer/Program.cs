using MoneyCategorizer.FlatCategorizer;
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
                Console.WriteLine($"Loaded {transactions.Count()} transactions from {directory}");

                var categorizer = new Categorizer(directory);
                var categorized = categorizer.Categorize2(transactions);
                Console.WriteLine($"Categorized {categorized.Count()} transactions");

                var manually = new ManuallySortedTransactions2();
                var sorted = manually.LoadAndAddNew(directory, categorized);
                Console.WriteLine($"Merged with existing transactions and got {sorted.Count()} transactions");

                var periods = GetPeriods();
                periods.ForEach(period => new Reporter().Report(Filter(sorted, period), period, directory));                
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

            for (int i = 0; i < 12; i++)
            {
                DateTime from = new DateTime(now.Year, now.Month, 1);
                DateTime to = from.AddMonths(1);
                periods.Add(new Period { From = from, To = to });
                now = now.AddMonths(-1);
            }

            periods.Add(new Period { From = new DateTime(DateTime.Now.Year, 1, 1), To = new DateTime(DateTime.Now.Year + 1, 1, 1) });
            periods.Add(new Period { From = new DateTime(DateTime.Now.Year - 1, 1, 1), To = new DateTime(DateTime.Now.Year, 1, 1) });


            return periods;            
        }

        static IEnumerable<Transaction> ReadAllTransactionsFromDirectory(string directory)
        {
            var dataProviders = new ITransactionProvider[] {
                new RbcCsvDataProvider(),
                new BoACreditCsvDataProvider(),
                new BoADebitCsvDataProvider(),                
                new Chase2022DebitCsvDataProvider(),
                new Chase2020CreditCsvDataProvider(),                
                new Chase2019CreditCsvDataProvider()
            };

            var transactions = new List<Transaction>();
            var fileUniqueness = new FileContentUniqueness();

            foreach (var file in Directory.EnumerateFiles(Path.Combine(directory,"bank"), "*.csv", SearchOption.AllDirectories))
            {
                try
                {
                    var fileContent = File.ReadAllText(file);

                    var dataProvider = (from x in dataProviders where x.FormatSupported(fileContent) select x).ToList();
                    if (dataProvider.Count != 1) throw new Exception($"For '{file}' need 1 data provider, but found: " + String.Join(",", dataProvider.Select(a=>a.GetType().Name)));
                                        
                    transactions.AddRange(dataProvider[0].GetTransactions(fileContent, file));
                }
                catch
                {
                    Console.WriteLine($"Failed for file {file}");
                    throw;
                }
            }

            fileUniqueness.AssertUnique(transactions, Path.Combine(directory, "user", "exemptions.txt"));

            return transactions;
        }

        static IEnumerable<SortedTransaction> Filter(IEnumerable<SortedTransaction> transactions, Period period)
        {
            return from t in transactions
                   where (period.From <= t.Date && t.Date < period.To)
                   select t;
        }
    }
}
