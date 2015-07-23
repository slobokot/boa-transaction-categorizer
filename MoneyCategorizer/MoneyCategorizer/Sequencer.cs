using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Sequencer
    {
        string s;
        int i;

        public Sequencer(string s)
        {
            this.s = s;
            i = 0;
        }

        public string GetNext()
        {
            int b = i;
            int e;
            if (s[i] == '"')
                e = s.IndexOf('"', i + 1) + 1;
            else
                e = s.IndexOf(',', i);
            
            if (e < 0)
                e = s.Length;
            
            i = e + 1;
            var result = s.Substring(b, e - b);
            return result;
        }
    }
}
