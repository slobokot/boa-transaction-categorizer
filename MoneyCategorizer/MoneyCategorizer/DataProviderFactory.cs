using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class DataProviderFactory
    {
        public IDataProvider GetDataProvider(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            if (lines[0].StartsWith("Description"))
                return new BoADebitCsvDataProvider(fileName);

            return new BoACreditCsvDataProvider(fileName);
        }
    }
}
