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
                var periods = new List<Period>();

                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                for (int i = 0; i < 6; i++)
                {
                    periods.Add(new Period { Year = now.Year, Month = now.Month });
                    now = now.AddMonths(-1);
                }

                var transactions = ReadAllTransactionsFromDirectory(directory);

                periods.ForEach(period => Run(transactions, period));                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        static IEnumerable<Transaction> ReadAllTransactionsFromDirectory(string directory)
        {
            var dataProviders = new ITransactionProvider[] {
                new BoACreditCsvDataProvider(),
                new BoADebitCsvDataProvider(),
                new ChaseCreditCsvDataProvider()
            };

            var transactions = new List<Transaction>();
            var fileUniqueness = new FileContentUniqueness();

            foreach (var file in Directory.EnumerateFiles(directory, "*.csv"))
            {
                var fileContent = File.ReadAllText(file);

                if (!fileUniqueness.IsUnique(fileContent))
                {
                    throw new Exception($"{file} has same content as some other file");
                }

                var dataProvider = dataProviders.First(provider => provider.FormatSupported(fileContent));                
                transactions.AddRange(dataProvider.GetTransactions(fileContent));                
            }
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
            var beginOfMonth = new DateTime(period.Year, period.Month, 01);
            var endOfMonth = new DateTime(period.Year, period.Month, 01).AddMonths(1);
            return from t in transactions
                   where (beginOfMonth <= t.Date && t.Date < endOfMonth)
                   select t;
        }
    }
}
