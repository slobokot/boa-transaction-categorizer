using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Reporter
    {
        public static void Report(IEnumerable<CategorizedTransaction> categorized)
        {
            var sorted = from c in categorized orderby c.Category select c;
            using (var sw = new StreamWriter("output.txt"))
            {
                PrintDetailed(sorted, sw);
                PrintSummary(sorted, sw);
            }
        }

        private static void PrintSummary(IEnumerable<CategorizedTransaction> categorized, TextWriter sw)
        {
            var total = 0.0;
            foreach (var category in categorized)
            {
                sw.WriteLine($"{category.Category}, {category.Amount}");                
                if (category.Category != WellKnownCategories.Income)
                    total += category.Amount;
            }
            sw.WriteLine($"Total spending,{total}");
        }

        private static void PrintDetailed(IEnumerable<CategorizedTransaction> categorized, TextWriter sw)
        {
            foreach (var category in categorized)
            {
                sw.WriteLine(category.Category + "," + category.Amount);
                foreach (var t in category.Transactions)
                    sw.WriteLine($"\t{t.Description} , {t.Amount}");
            }

            sw.WriteLine();
        }
    }
}
