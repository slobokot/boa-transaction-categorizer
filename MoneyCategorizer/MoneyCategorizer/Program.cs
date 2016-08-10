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

                foreach (var month in periods)
                    Run(directory, month);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        static void Run(string directory, Period period)
        {            
            if (File.GetAttributes(directory).HasFlag(FileAttributes.Directory))
            {
                var transactions = new List<Transaction>();

                var checkUniqueness = new HashSet<string>();
                var dataProviderFactory = new TransactionProviderFactory();
                foreach (var file in Directory.EnumerateFiles(directory, "*.csv"))
                {
                    var lines = File.ReadAllLines(file);
                    var dataProvider = dataProviderFactory.GetTransactionProvider(lines);
                    foreach (var line in dataProvider.GetBodyWithoutHeader())
                    {
                        if (checkUniqueness.Contains(line))
                        {
                            throw new Exception($"{file} has duplicate line {line}");
                        }
                        checkUniqueness.Add(line);
                    }                    

                    transactions.AddRange(dataProvider.GetTransactions());
                }

                var filteredTransactions = Filter(transactions, period);

                var categorizer = new Categorizer();
                var categorized = categorizer.Categorize(filteredTransactions);

                Reporter.Report(categorized, period);
            }
            else
            {
                throw new NotImplementedException();
            }
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
