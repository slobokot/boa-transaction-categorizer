using MoneyCategorizer.FlatCategorizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCategorizer
{
    class ReportTransactions
    {
        public string Category { get; set; }
        public List<SortedTransaction> Transactions { get; } = new List<SortedTransaction>();

        public static IEnumerable<ReportTransactions> From(IEnumerable<SortedTransaction> transactions)
        {
            var aggregate = new Dictionary<string, ReportTransactions>();
            foreach (var transaction in transactions)
            {
                var category = transaction.Category;
                if (category.Equals(WellKnownCategories.Exclude, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!aggregate.ContainsKey(category))
                {
                    aggregate.Add(category, new ReportTransactions { Category = category });
                }

                aggregate[category].Transactions.Add(transaction);
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
