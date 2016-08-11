using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    public class CsvLineParser
    {
        public string[] Parse(string line)
        {
            List<string> result = new List<string>();

            int i = 0;
            while(true)
            {
                if (i >= line.Length)
                    break;
                var r = GetNext(line, ref i);                
                result.Add(r);
            }

            if (line.EndsWith(","))
                result.Add("");

            return result.ToArray();
        }
                             
        string GetNext(string s, ref int i)
        {
            int b = i;
            int e;
            if (s[i] == '"')
            {
                b++;
                e = s.IndexOf('"', b);
                i = e + 2;
            }
            else
            {
                e = s.IndexOf(',', i);
                if (e < 0)
                    e = s.Length;
                i = e + 1;
            }
           
            var result = s.Substring(b, e - b);
            return result;
        }
    }
}
