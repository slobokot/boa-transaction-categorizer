using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoneyCategorizer
{
    class FileContentUniqueness
    {        
        class Merger
        {
            public List<Transaction> Transactions;                     

            public Merger(Transaction oldTransaction)
            {
                Transactions = new List<Transaction>();
                Transactions.Add(oldTransaction);                                
            }
        }

        public IEnumerable<Transaction> HandleNonUniqueCases(IEnumerable<Transaction> transactions, string exemptionFileName = null)
        {            
            var exemptions = ReadExemptions(exemptionFileName);
            var rawToMerger = new Dictionary<string, Merger>();

            var duplicates = new Dictionary<string, Merger>();
            foreach(var transaction in transactions)
            {
                if (rawToMerger.ContainsKey(transaction.Raw))
                {
                    rawToMerger[transaction.Raw].Transactions.Add(transaction);
                    if (!exemptions.Contains(transaction.Raw) && !duplicates.ContainsKey(transaction.Raw))
                    {                    
                        duplicates.Add(transaction.Raw, rawToMerger[transaction.Raw]);                        
                    }
                }
                else
                {                    
                    rawToMerger.Add(transaction.Raw, new Merger(transaction));
                }                
            }

            if (duplicates.Count > 0)
            {
                foreach(var entry in duplicates)
                {
                    Console.WriteLine(entry.Key);
                    foreach(var tx in entry.Value.Transactions)
                    {
                        Console.WriteLine("    " + tx.FileName);
                    }
                }
                throw new Exception($"There are {duplicates.Count} duplicate transactions\n");
            }
            var result = new List<Transaction>();
            foreach(var merger in rawToMerger)
            {                
                var i = 1;
                foreach (var tx in merger.Value.Transactions)
                {
                    if (i == 1)
                        result.Add(merger.Value.Transactions[0]);
                    else
                    {
                        var newtx = new Transaction();
                        newtx.Amount = tx.Amount;
                        newtx.Date = tx.Date;
                        newtx.Description = tx.Description;
                        newtx.FileName = tx.FileName;
                        newtx.Raw = $"{tx.Raw}[{i}]";
                        result.Add(newtx);
                    }
                    i++;
                }                
            }
            return result;
        }

        private HashSet<string> ReadExemptions(string exemptionFileName = null)
        {            
            var lines = File.ReadAllLines(exemptionFileName);
            HashSet<string> exemption = new HashSet<string>();
            foreach(var line in lines)
            {
                exemption.Add(line.Trim());
            }
            return exemption;
        }
    }
}
