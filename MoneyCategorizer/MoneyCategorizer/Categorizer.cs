using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace MoneyCategorizer
{
    class Categorizer
    {
        // category -> list of templates
        Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();

        public Categorizer()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "Categories.json").Replace("file:\\","");
            if (File.Exists(path))
                categories = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(path));
            else     
                categories = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Properties.Resources.Categories);
        }

        public IEnumerable<CategorizedTransaction> Categorize(IEnumerable<Transaction> transactions)
        {            
            var aggregate = new Dictionary<string, CategorizedTransaction>();
            foreach (var transaction in transactions)
            {                
                var category = GetCategory(transaction);
                if (category.Equals(WellKnownCategories.Exclude, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!aggregate.ContainsKey(category))          
                    aggregate.Add(category, new CategorizedTransaction { Category = category });

                aggregate[category].Transactions.Add(transaction);
            }

            foreach(var transaction in aggregate.Values)            
                transaction.Transactions.Sort((x, y) => x.Date.CompareTo(y.Date));
            
            return aggregate.Values;
        }

        private string GetCategory(Transaction transaction)
        {
            foreach(var categoryList in categories)
            {
                foreach(var categoryTemplate in categoryList.Value)
                if (transaction.Description.ToLower().Contains(categoryTemplate.ToLower()))
                {
                    return categoryList.Key;
                }
            }

            return WellKnownCategories.Unknown;
        }
    }
}
