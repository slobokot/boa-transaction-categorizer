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
                string directory = args[0];
                var monthes = new List<int>();
                if (args.Length == 2)
                    monthes.Add(int.Parse(args[1]));
                else
                    for (int i = 0; i < 3; i++)
                        monthes.Add(DateTime.Now.Month - i);

                foreach (var month in monthes)
                    Run(directory, month);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Run(string directory, int month)
        {            
            FileAttributes attr = File.GetAttributes(directory);
            if(attr.HasFlag(FileAttributes.Directory))
            {
                List<Transaction> transactions = new List<Transaction>();

                var dataProviderFactory = new TransactionProviderFactory();
                foreach (var file in Directory.EnumerateFiles(directory))
                {
                    var lines = File.ReadAllLines(file);
                    var dataProvider = dataProviderFactory.GetTransactionProvider(lines);  
                    transactions.AddRange(dataProvider.GetTransactions()); 
                }

                var filteredTransactions = Filter(transactions, DateTime.Now.Year, month);
                if (filteredTransactions.Count() == 0)                
                    filteredTransactions = Filter(transactions, DateTime.Now.Year - 1, month);
                
                var categorizer = new Categorizer();
                var categorized  = categorizer.Categorize(filteredTransactions);

                Reporter.Report(categorized, month);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        static IEnumerable<Transaction> Filter(IEnumerable<Transaction> transactions, int year, int month)
        {
            var beginOfMonth = new DateTime(year, month, 01);
            var endOfMonth = new DateTime(year, month, 01).AddMonths(1);
            return from t in transactions
                   where (t.Date >= beginOfMonth && t.Date < endOfMonth)
                   select t;
        }
    }
}
