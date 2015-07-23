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

            foreach (var line in File.ReadAllLines(fileName).Skip(1))
            {
                var s = new Sequencer(line);

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
