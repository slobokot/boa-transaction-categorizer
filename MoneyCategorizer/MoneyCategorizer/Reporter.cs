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
        public static void Report(IEnumerable<CategorizedTransaction> categorized, Period period)
        {
            var sorted = from c in categorized orderby c.Category select c;
            using (var sw = new StreamWriter($"output_{period.Year}_{period.Month}.txt"))
            {
                PrintDetailed(sorted, sw);
                PrintSummary(sorted, sw);
            }
        }

        private static void PrintSummary(IEnumerable<CategorizedTransaction> categorized, TextWriter sw)
        {
            var totalSpending = 0.0;
            var totalIncome = 0.0;
            foreach (var category in categorized)
            {
                sw.WriteLine($"{category.Category}, {category.Amount}");
                if (category.Category != WellKnownCategories.Income)
                    totalSpending += category.Amount;
                else
                    totalIncome += category.Amount;
            }
            sw.WriteLine();
            sw.WriteLine($"Total spending, {totalSpending}");
            sw.WriteLine($"Total saving, {totalIncome+totalSpending}");
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
