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
            string filename;
            if ((period.To - period.From).TotalDays < 32)
            {                
                filename = $"output_{period.From.ToString("yyyy_MM")}.txt";
            }
            else
            {
                filename = $"output_{period.From.ToString("yyyy_MM_dd")}-{period.To.ToString("yyyy_MM_dd")}.txt";
            }

            using (var sw = new StreamWriter(filename))
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
                sw.WriteLine($"{category.Category}, {category.Amount.ToString("0.00")}");
                if (category.Category != WellKnownCategories.Income)
                    totalSpending += category.Amount;
                else
                    totalIncome += category.Amount;
            }
            sw.WriteLine();
            sw.WriteLine($"Total spending, {totalSpending.ToString("0.00")}");
            sw.WriteLine($"Total saving, {(totalIncome+totalSpending).ToString("0.00")}");
        }

        private static void PrintDetailed(IEnumerable<CategorizedTransaction> categorized, TextWriter sw)
        {
            foreach (var category in categorized)
            {
                sw.WriteLine(category.Category + "," + category.Amount.ToString("0.00"));
                foreach (var t in from c in category.Transactions orderby Math.Abs(c.Amount) select c)
                    sw.WriteLine($"\t{t.Description} , {t.Amount.ToString("0.00")}");
            }

            sw.WriteLine();
        }
    }
}
