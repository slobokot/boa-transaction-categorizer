using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    static class DataProviderExtensions
    {
        public static void CheckFormatSupported(this ITransactionProvider transactionProvider, string fileContent)
        {
            if (!transactionProvider.FormatSupported(fileContent))
            {
                throw new Exception("File format is not supported");
            }
        }

        public static string[] SplitStringIntoLines(string content)
        {
            return content.Replace("\r","").Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
