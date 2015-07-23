using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Reporter
    {
        public static void Report(IEnumerable<CategorizedTransaction> categorized)
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
