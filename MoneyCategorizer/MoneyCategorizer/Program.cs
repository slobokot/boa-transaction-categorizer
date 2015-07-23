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
            FileAttributes attr = File.GetAttributes(args[0]);
            if(attr.HasFlag(FileAttributes.Directory))
            {
                List<Transaction> transactions = new List<Transaction>();

                var dataProviderFactory = new TransactionProviderFactory();
                foreach (var file in Directory.EnumerateFiles(args[0]))
                {
                    var lines = File.ReadAllLines(file);
                    var dataProvider = dataProviderFactory.GetTransactionProvider(lines);  
                    transactions.AddRange(dataProvider.GetTransactions()); 
                }

                var filteredTransactions = from t in transactions
                                            where t.Date >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01)
                                            select t;

                var categorizer = new Categorizer();
                var categorized  = categorizer.Categorize(filteredTransactions);

                Reporter.Report(categorized);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
