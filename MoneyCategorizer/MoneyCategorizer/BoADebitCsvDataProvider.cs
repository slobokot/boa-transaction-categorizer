using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class BoADebitCsvDataProvider : IDataProvider
    {
        string fileName;

        public BoADebitCsvDataProvider(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<Transaction> GetTransactions()
        {           
            var lines = File.ReadAllLines(fileName);
            if (!lines[0].Equals("Description,,Summary Amt.") ||
                !lines[6].Equals("Date,Description,Amount,Running Bal.") ||
                !lines[7].Contains("Beginning balance as of"))

                throw new Exception(fileName + ": " + lines[0] + ". " + lines[6]);
            
            var result = new List<Transaction>();

            foreach (var line in lines.Skip(8))
            {                                                   
                var sequencer = new Sequencer(line);
                
                var transaction = new Transaction();
                transaction.Date = DateTime.ParseExact(sequencer.GetNext(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                transaction.Description = sequencer.GetNext().Trim('"');
                transaction.Place = "";                
                
                transaction.Amount = double.Parse(sequencer.GetNext().Trim('"'));
                result.Add(transaction);
            }

            return result;
        }
    }
}
