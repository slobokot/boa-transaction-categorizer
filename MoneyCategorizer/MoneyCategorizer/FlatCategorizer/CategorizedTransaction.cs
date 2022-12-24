using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer.FlatCategorizer
{
    public class CategorizedTransaction
    {
        public Transaction Transaction { get;set; }
        public string Category { get; set; }       
        public string ExtraDescription { get; set; } 
    }
}
