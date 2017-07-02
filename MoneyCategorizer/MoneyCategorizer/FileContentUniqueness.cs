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

            foreach(var transaction in transactions)
            {
                if (checkUniqueness.Contains(transaction.Raw))
                {
                    if (!exemptions.Contains(transaction.Raw))
                    {
                        throw new Exception($"{transaction.Raw} is duplicated");
                    }
                }
                else
                {
                    checkUniqueness.Add(transaction.Raw);
                }
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
