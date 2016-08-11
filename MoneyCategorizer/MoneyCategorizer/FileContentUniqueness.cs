using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class FileContentUniqueness
    {
        HashSet<string> checkUniqueness = new HashSet<string>();

        public bool IsUnique(string fileContent)
        {
            var fileStamp = fileContent;
            if (fileStamp.Length > 300)
            {
                fileStamp = fileStamp.Substring(0, 300);
            }

            if (checkUniqueness.Contains(fileStamp))
            {
                return false;
            }

            checkUniqueness.Add(fileStamp);
            return true;
        }
    }
}
