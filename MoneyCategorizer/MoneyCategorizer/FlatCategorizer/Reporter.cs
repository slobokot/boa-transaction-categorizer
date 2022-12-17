using MoneyCategorizer.FlatCategorizer;
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
        double total = double.NaN;
        double totalSpending = double.NaN;
        double totalIncome = double.NaN;

        public void Report(IEnumerable<CategorizedTransaction> categorized, Period period, string root)
        {
            var sorted = CategorizedTransactions.From(categorized);
            string filename;
            
            if ((period.To - period.From).TotalDays < 32)
            {                
                filename = $"output_{period.From.ToString("yyyy_MM")}.txt";            
            }
            else
            {
                filename = $"output_{period.From.ToString("yyyy_MM_dd")}-{period.To.ToString("yyyy_MM_dd")}.txt";                
            }

            PreCalculateNumbers(sorted);            
            using (var sw = new StreamWriter(Path.Combine(root, filename)))
            {
                PrintSummary(sorted, sw);
                PrintDetailed(sorted, sw);               
            }
        }

        private void PreCalculateNumbers(IEnumerable<CategorizedTransactions> categorized)
        {
            total = 0.0;
            totalSpending = 0.0;
            totalIncome = 0.0;
            
            foreach (var category in from c in categorized orderby c.Amount select c)
            {
                total += category.Amount;
                //if (category.Category != WellKnownCategories.Income)
                if (category.Amount <= 0)
                    totalSpending += category.Amount;
                else
                    totalIncome += category.Amount;
            }
        }

        private void PrintSummary(IEnumerable<CategorizedTransactions> categorized, TextWriter sw)
        {
            sw.WriteLine("--------------------------------");
            
            foreach (var category in from c in categorized orderby c.Amount select c)
            {                
                sw.WriteLine(category.Category + ", sum " + category.Amount.ToString("0.00") + ", p " + category.PositiveAmountSum + ", m " + category.NegativeAmountSum + ", " + percentage(category).ToString("0") + "%");                
            }
            sw.WriteLine();
            sw.WriteLine($"Total income\t{totalIncome.ToString("0.00")}");
            sw.WriteLine($"Total spending\t{totalSpending.ToString("0.00")}");
            sw.WriteLine($"Total saving\t{(totalIncome+totalSpending).ToString("0.00")}");
        }

        private double percentage(CategorizedTransactions category)
        {
            return category.Amount * 100 / (category.Amount < 0 ? totalSpending : totalIncome);
        }

        private void PrintDetailed(IEnumerable<CategorizedTransactions> categorized, TextWriter sw)
        {
            sw.WriteLine("--------------------------------");
            foreach (var category in from c in categorized orderby c.Amount select c)
            {
                sw.WriteLine(category.Category + ", sum " + category.Amount.ToString("0.00") + ", p " + category.PositiveAmountSum + ", m " + category.NegativeAmountSum + ", " + percentage(category).ToString("0") + "%");
                foreach (var t in from c in category.Transactions orderby Math.Abs(c.Amount) select c)
                    sw.WriteLine($"\t{t.Description} {t.Date.ToString("MMM-dd")}\t{t.Amount.ToString("0.00")}");
                sw.WriteLine();
            }

            sw.WriteLine();
        }
    }
}
