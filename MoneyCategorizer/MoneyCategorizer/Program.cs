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
            var dataProviderFactory = new DataProviderFactory();
            var categorizer = new Categorizer();

            FileAttributes attr = File.GetAttributes(args[0]);
            if(attr.HasFlag(FileAttributes.Directory))
            {
                List<Transaction> transactions = new List<Transaction>();

                foreach(var file in Directory.EnumerateFiles(args[0]))
                {
                    var dataProvider = dataProviderFactory.GetDataProvider(file);                    
                    transactions.AddRange(dataProvider.GetTransactions()); 
                }

                var categorized  = categorizer.Categorize(
                    (from t in transactions 
                    where t.Date >= new DateTime(2015, 07, 01)
                    select t).ToArray()
                    );

                Print(categorized);
            }
        }

        static void Print(IEnumerable<CategorizedTransaction> categorized)
        {
            PrintDetailed(categorized);
            PrintSummary(categorized);
        }

        private static void PrintSummary(IEnumerable<CategorizedTransaction> categorized)
        {
            var total = 0.0;
            foreach (var category in categorized)
            {
                Console.WriteLine($"{category.Category}, {category.Amount}");
                if (category.Category != Category.Income)
                    total += category.Amount;
            }
            Console.WriteLine($"Total spending,{total}");
        }

        private static void PrintDetailed(IEnumerable<CategorizedTransaction> categorized)
        {
            foreach (var category in categorized)
            {
                Console.WriteLine(category.Category + "," + category.Amount);
                foreach (var t in category.Transactions)
                    Console.WriteLine($"\t{t.Description} , {t.Amount}");
            }

            Console.WriteLine();
        }
    }
}
