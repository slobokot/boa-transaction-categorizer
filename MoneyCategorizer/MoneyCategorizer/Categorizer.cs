using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace MoneyCategorizer
{
    class Categorizer
    {
        // category -> list of templates
        Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
        Dictionary<string, List<Regex>> regexCategories = new Dictionary<string, List<Regex>>();

        public Categorizer()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "Categories.json").Replace("file:\\","");
            if (!File.Exists(path))
            {
                throw new Exception($"File {path} does not exist");
            }
            categories = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(path));
            foreach(var category in categories)
            {
                for(int i = category.Value.Count - 1; i >= 0 ; i--)
                {
                    if (category.Value[i].StartsWith("/"))
                    {
                        if (!regexCategories.ContainsKey(category.Key))
                        {
                            regexCategories.Add(category.Key, new List<Regex>());
                        }
                        regexCategories[category.Key].Add(new Regex(category.Value[i].Substring(1), RegexOptions.IgnoreCase));
                        category.Value.RemoveAt(i);
                    }
                }
            }
        }

        public IEnumerable<CategorizedTransaction> Categorize(IEnumerable<Transaction> transactions)
        {            
            var aggregate = new Dictionary<string, CategorizedTransaction>();
            foreach (var transaction in transactions)
            {                
                var category = GetCategory(transaction);
                if (category.Equals(WellKnownCategories.Exclude, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!aggregate.ContainsKey(category))
                {
                    aggregate.Add(category, new CategorizedTransaction { Category = category });
                }

                aggregate[category].Transactions.Add(transaction);
            }

            foreach (var transaction in aggregate.Values)
            {
                transaction.Transactions.Sort((x, y) => x.Date.CompareTo(y.Date));
            }
            
            return aggregate.Values;
        }

        private string GetCategory(Transaction transaction)
        {
            foreach (var categoryList in categories)
            {
                foreach (var categoryTemplate in categoryList.Value)
                {
                    if (transaction.Description.ToLower().Contains(categoryTemplate.ToLower()))
                    {
                        return categoryList.Key;
                    }
                }
            }
            foreach (var categoryList in regexCategories)
            {
                foreach(var categoryRegex in categoryList.Value)
                {
                    if (categoryRegex.IsMatch(transaction.Description))
                    {
                        return categoryList.Key;
                    }
                }
            }

            return WellKnownCategories.Unknown;
        }
    }
}
