using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace MoneyCategorizer.FlatCategorizer
{
    public class ManuallySortedTransactions2
    {
        long offset = ((DateTimeOffset)new DateTime(2020, 01, 01)).ToUnixTimeSeconds();
        long previousId = 0;
        int subId = 0;        
        string header = "Id, Date, Description, Amount, Category";
                
        public List<SortedTransaction> LoadAndAddNew(string root, IEnumerable<CategorizedTransaction> cts)
        {
            var transactions = LoadExistingTransactions(root);
            Console.WriteLine($"Loaded {transactions.Count()} sorted transactions");

            var rawToId = LoadExistingRawToIds(root);

            return MergeAndSave(root, cts, rawToId, transactions);
        }

        private Dictionary<string, string> LoadExistingRawToIds(string root)
        {
            var result = new Dictionary<string, string>();                        
            int lineNumber = -1;
            var fileName = "n/a";
            try
            {
                lineNumber = 1;
                fileName = GetIdsFileName(root);
                if (File.Exists(fileName))
                {
                    var lines = File.ReadAllLines(fileName);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var idx = line.IndexOf("=");
                        var id = line.Substring(0, idx);
                        var raw = line.Substring(idx + 1);
                        result.Add(raw, id);
                        lineNumber++;
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Failed for file {fileName} at line {lineNumber}");
                throw;
            }
            
            return result;
        }

        private List<SortedTransaction> LoadExistingTransactions(string root)
        {
            var transactions = new List<SortedTransaction>();

            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "user"), "sorted*.csv", SearchOption.AllDirectories))
            {
                int lineNumber = -1;
                var currentLine = "";
                string[] parsed = null;
                try
                {
                    lineNumber = 1;
                    var lines = File.ReadAllLines(file);
                    
                    foreach (var line in lines)
                    {
                        if (lineNumber != 1)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                                continue;
                            currentLine = line;
                            string rawTransaction = line;
                            CsvLineParser parser = new CsvLineParser();
                            parsed = parser.Parse(line);
                            var sortedTransaction = new SortedTransaction();
                            sortedTransaction.Id = parsed[0];
                            sortedTransaction.Date = DateTime.ParseExact(parsed[1], "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            sortedTransaction.Description = parsed[2];
                            sortedTransaction.Amount = double.Parse(parsed[3]);
                            sortedTransaction.Category = string.IsNullOrWhiteSpace(parsed[4]) ? WellKnownCategories.Unknown : parsed[4];
                            transactions.Add(sortedTransaction);
                        }
                        lineNumber++;
                    }
                }
                catch
                {
                    Console.WriteLine($"Failed for file {file} at line {lineNumber}:");
                    Console.WriteLine(currentLine);
                    if (parsed != null) foreach (var x in parsed) Console.WriteLine(x);
                    throw;
                }
            }
            return transactions;
        }
       
        private List<SortedTransaction> MergeAndSave(string root, 
            IEnumerable<CategorizedTransaction> cts,
            Dictionary<string, string> rawToId,
            List<SortedTransaction> existing
            )
        {
            var result = new List<SortedTransaction>();
            result.AddRange(existing);
            var newRawToId = new Dictionary<string, string>();
            foreach (var entry in rawToId)
                newRawToId.Add(entry.Key, entry.Value);

            var monthToTrans = new Dictionary<string, List<SortedTransaction>>();// date - list<categorized>
            foreach (var ct in 
                cts.Where(x => !rawToId.ContainsKey(x.Transaction.Raw))
                .OrderBy(x => x.Transaction.Date))// to keep aggregate list sorted
            {
                var date = ct.Transaction.Date.ToString("yyyy-MM");
                if (!monthToTrans.ContainsKey(date))
                    monthToTrans.Add(date, new List<SortedTransaction>());

                var sortedTransaction = new SortedTransaction();
                sortedTransaction.Amount = ct.Transaction.Amount;
                sortedTransaction.Category = ct.Category;
                sortedTransaction.Date = ct.Transaction.Date;
                sortedTransaction.Description = ct.Transaction.Description +
                    (!string.IsNullOrWhiteSpace(ct.ExtraDescription) ? (' ' + ct.ExtraDescription) : string.Empty);
                sortedTransaction.Id = GetId(ct, newRawToId);

                monthToTrans[date].Add(sortedTransaction);
                result.Add(sortedTransaction);
            }
            
            foreach (var entry in monthToTrans)
            {
                var fileName = Path.Combine(root, "user", $"sorted-{entry.Key}.csv");
                var headerExist = File.Exists(fileName);
                using (StreamWriter sw = new StreamWriter(fileName, true))
                {
                    if (!headerExist)
                        sw.WriteLine(header);
                    foreach (var rawTransaction in entry.Value)
                    {
                        if (rawTransaction.Description.Contains('"')) throw new Exception(rawTransaction.Id + " has a quote in description");
                        sw.WriteLine(rawTransaction.Id + "," + 
                            rawTransaction.Date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + "," +
                            "\"" + rawTransaction.Description + "\"," + 
                            rawTransaction.Amount.ToString("0.00") + "," +
                            ((rawTransaction.Category == WellKnownCategories.Unknown) ? "" : rawTransaction.Category) );
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(GetIdsFileName(root)))
            {
                foreach (var entry in from x in newRawToId orderby x.Value select x)
                {
                    sw.WriteLine(entry.Value + "=" + entry.Key);
                }
            }

            return result;
        }
        
        private string GetIdsFileName(string root)
        {
            return Path.Combine(root, "user", $"ids.csv");
        }

        private string GetId(CategorizedTransaction ct, Dictionary<string, string> rawToId)
        {           
            if (!rawToId.ContainsKey(ct.Transaction.Raw))
            {
                string newIdStr;
                long newId = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() - offset;
                if (newId == previousId)
                {
                    subId++;
                    newIdStr = newId + "x" + subId;
                }
                else
                {
                    subId = 0;
                    previousId = newId;
                    newIdStr = newId.ToString();
                }
                rawToId[ct.Transaction.Raw] = newIdStr;
            }

            return rawToId[ct.Transaction.Raw];
        }        
    }
}