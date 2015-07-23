using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class Categorizer
    {
        Dictionary<string, Category> categories = new Dictionary<string, Category> () { 
        { "KROGER", Category.Food } ,
        { "WAL-MART", Category.Food },
        { "WM SUPERCENTER", Category.Food },
        { "KHANS MONGOLIAN GRILL", Category.Food},
        { "PUBLIX", Category.Food },
        { "THE GREEK TOUCH NASHVILLE",Category.Food},
        { "ALEKSEYS MARKET NASHVILLE TN",Category.Food},

        { "WALGREENS", Category.Health },

        { "SHELL OIL", Category.Gas },
        { "EXXONMOBIL", Category.Gas },

        { "ADVANCE AUTO PARTS", Category.Car },

        { "MCDONALD'S", Category.Restaurant },
        { "RED LOBSTER", Category.Restaurant },
        { "STEAK HOUSE", Category.Restaurant },
        { "LOGANS", Category.Restaurant },

        { "GYMBOREE", Category.Baby },

        { "TARGET", Category.HouseImprovement },

        { "Arbors of Brentw", Category.Apartment },
        { "APPLIANCE WAREHOU", Category.Apartment },
        { "CRICKET WIRELESS", Category.Apartment },
        { "COMCAST OF NASHVILLE", Category.Apartment },
        { "RENT INS PROPCAS", Category.Apartment },

        { "Online payment from CHK", Category.Exclude },
        { "Beginning balance as of", Category.Exclude },
        { "Online Banking payment to CRD 2868 Confirmation#", Category.Exclude },

        { "PRIMEPOINT LLC DES:PAYROLL ID", Category.Income}
        };

        public IEnumerable<CategorizedTransaction> Categorize(IEnumerable<Transaction> transactions)
        {
            var aggregate = new Dictionary<Category, List<Transaction>>();
            foreach (var transaction in transactions)
            {                
                var category = GetCategory(transaction);
                if (category == Category.Exclude)
                    continue;

                if (!aggregate.ContainsKey(category))                
                    aggregate.Add(category, new List<Transaction>());

                aggregate[category].Add(transaction);
            }

            return from x in aggregate select new CategorizedTransaction { Category = x.Key, Transactions = x.Value };
        }

        private Category GetCategory(Transaction transaction)
        {
            foreach(var category in categories)
            {
                if (transaction.Description.ToLower().Contains(category.Key.ToLower()))
                {
                    return category.Value;
                }
            }

            return Category.Unknown;
        }
    }
}
