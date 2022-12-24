using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using MoneyCategorizer.FlatCategorizer;
using MoneyCategorizer;

namespace MoneyCategorizer
{
    class Categorizer
    {
        // category -> list of templates
        Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
        Dictionary<string, List<Regex>> regexCategories = new Dictionary<string, List<Regex>>();        

        public Categorizer(string root)
        {            
            var path = Path.Combine(root, "user", "Categories.json");
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

        public IEnumerable<CategorizedTransaction> Categorize2(IEnumerable<Transaction> transactions)
        {
            var result = new List<CategorizedTransaction>();
            foreach (var transaction in transactions)
            {                                                                
                var toAdd = new CategorizedTransaction{ Transaction = transaction, Category = GetCategory(transaction) };
                
                if (!toAdd.Category.Equals(WellKnownCategories.Exclude, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Add(toAdd);
                }
            }

            return result;
        }

        private string GetCategory(Transaction transaction)
        {            
            foreach (var categoryEntry in categories)
            {
                foreach (var categoryTemplate in categoryEntry.Value)
                {
                    if (transaction.Description.ToLower().Contains(categoryTemplate.ToLower()))
                    {
                        return categoryEntry.Key;                        
                    }
                }
            }

            foreach (var categoryEntry in regexCategories)
            {
                foreach(var categoryRegex in categoryEntry.Value)
                {
                    if (categoryRegex.IsMatch(transaction.Description))
                    {
                        return categoryEntry.Key;                        
                    }
                }
            }

            return WellKnownCategories.Unknown;
        }
    }
}
