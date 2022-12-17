using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MoneyCategorizer.FlatCategorizer
{
    public class ManuallySortedTransactions
    {
        Dictionary<string, Details> rawTransactionToCategory = new Dictionary<string, Details>();        
        Dictionary<char, string> categories = new Dictionary<char, string>();

        class Details
        {
            public string Category { get; set; }
            public string ExtraDescription { get; set; }
        }

        public ManuallySortedTransactions()
        {            
            categories.Add('l', "Lota");
            categories.Add('s', "Sergiy");
            categories.Add('m', "Marat");
            categories.Add('e', "Evelyne");            
            categories.Add('a', "ExpensedAmazon");
        }

        public void Load(string root)
        {            
            var transactions = new List<Transaction>();
            int lineNumber = -1;
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "user"), "custom*.csv", SearchOption.AllDirectories))
            {
                try
                {
                    var lines = File.ReadAllLines(file);
                    lineNumber = 1;
                    foreach(var line in lines)
                    {
                        string rawTransaction = line;
                        Details details = new Details() { Category = WellKnownCategories.Unknown, ExtraDescription = string.Empty };
                        /* valid lines
                         * COFFEE 35.00,``s
                         * COFFEE 35.00,``s birthday
                         * COFFEE 35.00,```Sergiy
                         * COFFEE 35.00,```Sergiy birthday
                         */
                        if (line.Contains("``"))
                        {
                            var i = line.IndexOf("``");
                            rawTransaction = line.Substring(0, i);
                            var lastChar = line[i+2];
                            if (lastChar == '`')
                            {
                                var i2 = line.IndexOf(' ', i + 3);
                                if (i2 < 0)
                                {
                                    details.Category = line.Substring(i + 3);
                                }
                                else
                                {
                                    details.Category = line.Substring(i + 3, i2 - i - 3);
                                    details.ExtraDescription = line.Substring(i2);
                                }
                            }
                            else
                            {
                                if (!categories.ContainsKey(lastChar))
                                    throw new Exception($"unknown category: {lastChar} at line {lineNumber}: context {line}");

                                details.Category = categories[lastChar];
                                if (line.Length > i + 3)
                                    details.ExtraDescription = line.Substring(i + 3);
                            }
                        }

                        if (!rawTransactionToCategory.ContainsKey(rawTransaction))
                        {
                            rawTransactionToCategory.Add(rawTransaction, details);
                        }
                        lineNumber++;
                    }
                }
                catch
                {
                    Console.WriteLine($"Failed for file {file} at line {lineNumber}");
                    throw;
                }
            }            
        }
       
        public void Save(string root, IEnumerable<CategorizedTransaction> transactions)
        {
            var aggregate = new Dictionary<string, List<string>>();
            foreach (var transaction in 
                transactions.Where(x => x.Category == WellKnownCategories.Unknown && 
                !rawTransactionToCategory.ContainsKey(x.Transaction.Raw))
                .OrderBy(x => x.Transaction.Date))
            {
                var date = transaction.Transaction.Date.ToString("yyyy-MM");
                if (!aggregate.ContainsKey(date))
                    aggregate.Add(date, new List<string>());
                aggregate[date].Add(transaction.Transaction.Raw);
            }
            foreach (var entry in aggregate)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(root, "user", $"custom-{entry.Key}.csv"), 
                    true))
                {
                    foreach (var rawTransaction in entry.Value)
                    {
                        sw.WriteLine(rawTransaction);
                    }
                }
            }
        }

        public IEnumerable<CategorizedTransaction> GetCategory(Transaction transaction)
        {
            Details result;
            if (rawTransactionToCategory.TryGetValue(transaction.Raw, out result))
            {
                if (result.Category != WellKnownCategories.Unknown)
                {
                    transaction.Description += result.ExtraDescription;
                    return new OneElementList<CategorizedTransaction>(new CategorizedTransaction { Transaction = transaction, Category = result.Category });
                }
            }

            return Enumerable.Empty<CategorizedTransaction>();
        }        
    }
}