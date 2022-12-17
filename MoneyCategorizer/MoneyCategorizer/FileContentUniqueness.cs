using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoneyCategorizer
{
    class FileContentUniqueness
    {        
        public void AssertUnique(IEnumerable<Transaction> transactions, string exemptionFileName = null)
        {
            var exemptions = ReadExemptions(exemptionFileName);
            var checkUniqueness = new Dictionary<string, List<string>>();

            var duplicates = new Dictionary<string, List<string>>();
            foreach(var transaction in transactions)
            {
                if (checkUniqueness.ContainsKey(transaction.Raw))
                {
                    if (!exemptions.Contains(transaction.Raw))
                    {
                        if (!duplicates.ContainsKey(transaction.Raw))
                        {
                            duplicates.Add(transaction.Raw, new List<string>());
                        }
                        duplicates[transaction.Raw].Add(transaction.FileName);
                        duplicates[transaction.Raw].AddRange(checkUniqueness[transaction.Raw]);
                    }
                }
                else
                {
                    var list = new List<string>();
                    list.Add(transaction.FileName);
                    checkUniqueness.Add(transaction.Raw, list);
                }
            }

            if (duplicates.Count > 0)
            {
                foreach(var entry in duplicates)
                {
                    Console.WriteLine(entry.Key);
                    foreach(var file in entry.Value)
                    {
                        Console.WriteLine("    " + file);
                    }
                }
                throw new Exception($"There are {duplicates.Count} duplicate transactions\n");
            }
        }

        private HashSet<string> ReadExemptions(string exemptionFileName = null)
        {            
            var lines = File.ReadAllLines(exemptionFileName);
            HashSet<string> excemptions = new HashSet<string>();
            foreach(var line in lines)
            {
                excemptions.Add(line.Trim());
            }
            return excemptions;
        }
    }
}
