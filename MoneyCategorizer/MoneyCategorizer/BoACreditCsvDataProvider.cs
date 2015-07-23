using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class BoACreditCsvDataProvider : IDataProvider
    {
        string fileName;

        public BoACreditCsvDataProvider(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            var result = new List<Transaction>();
            var lines = File.ReadAllLines(fileName);

            if (!lines[0].Equals("Posted Date,Reference Number,Payee,Address,Amount"))
                throw new Exception(fileName);

            foreach (var line in lines.Skip(1))
            {
                var s = new Sequencer(line);
                "Posted Date,Reference Number,Payee,Address,Amount"
                var transaction = new Transaction();
                transaction.Date = DateTime.ParseExact(s.GetNext(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                s.GetNext();
                transaction.Description = s.GetNext().Trim('"');
                transaction.Place = s.GetNext().Trim('"');
                transaction.Amount = double.Parse(s.GetNext());
                result.Add(transaction);
            }

            return result;
        }
    }
}
