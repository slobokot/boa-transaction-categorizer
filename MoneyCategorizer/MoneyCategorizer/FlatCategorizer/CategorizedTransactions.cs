using MoneyCategorizer.FlatCategorizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class CategorizedTransactions
    {
        public string Category { get; set; }
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        public static IEnumerable<CategorizedTransactions> From(IEnumerable<CategorizedTransaction> transactions)
        {
            var aggregate = new Dictionary<string, CategorizedTransactions>();
            foreach (var transaction in transactions)
            {
                var category = transaction.Category;
                if (category.Equals(WellKnownCategories.Exclude, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!aggregate.ContainsKey(category))
                {
                    aggregate.Add(category, new CategorizedTransactions { Category = category });
                }

                aggregate[category].Transactions.Add(transaction.Transaction);
            }

            foreach (var transaction in aggregate.Values)
            {
                transaction.Transactions.Sort((x, y) => x.Date.CompareTo(y.Date));
            }

            return aggregate.Values;
        }

        public double Amount
        {
            get { return Transactions.Sum(x => x.Amount); }
        }

        public double NegativeAmountSum
        {
            get { return Transactions.Where(x => x.Amount < 0).Sum(x => x.Amount); }
        }

        public double PositiveAmountSum
        {
            get { return Transactions.Where(x => x.Amount > 0).Sum(x => x.Amount); }
        }
    }
}
