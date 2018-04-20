using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoneyCategorizer
{
    class FileContentUniqueness
    {        
        public void AssertUnique(IEnumerable<Transaction> transactions)
        {
            var exemptions = ReadExemptions();
            var checkUniqueness = new HashSet<string>();

            var duplicates = new HashSet<string>();
            foreach(var transaction in transactions)
            {
                if (checkUniqueness.Contains(transaction.Raw))
                {
                    if (!exemptions.Contains(transaction.Raw))
                    {
                        if (!duplicates.Contains(transaction.Raw))
                        {
                            duplicates.Add(transaction.Raw);
                        }                        
                    }
                }
                else
                {
                    checkUniqueness.Add(transaction.Raw);
                }
            }

            if (duplicates.Count > 0)
            {
                throw new Exception($"There are {duplicates.Count} duplicate transactions\n" + string.Join("\n", duplicates));
            }
        }

        private HashSet<string> ReadExemptions()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "exemptions.txt").Replace("file:\\", "");
            var lines = File.ReadAllLines(path);
            HashSet<string> excemptions = new HashSet<string>();
            foreach(var line in lines)
            {
                excemptions.Add(line.Trim());
            }
            return excemptions;
        }
    }
}
