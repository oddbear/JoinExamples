using JoinExamples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoinExamples
{
    public static class NormalizationExtensions
    {
        public static TResult NormalizeToSingle<TEntity, TResult>(this IEnumerable<TEntity> collection, Func<TEntity, TResult> lambda)
            where TEntity : Entity
        {
            var norm = Normalize(collection, lambda); //To Single because of grouping!
            return norm.SingleOrDefault();
        }
        public static IEnumerable<TResult> Normalize<TEntity, TResult>(this IEnumerable<TEntity> collection, Func<TEntity, TResult> lambda)
            where TEntity : Entity
        {
            var arr = collection.Select(lambda);
            var norm = arr.Cast<Entity>().AsNormalized().Cast<TResult>(); //Groups and Merge.
            return norm.ToArray();
        }

        public static IEnumerable<TEntity> AsNormalized<TEntity>(this IEnumerable<TEntity> collection) //Mutable, will change state!
            where TEntity : Entity
        {
            var arr = collection.Where(c => c != null).ToArray();
            var group = arr.GroupBy(e => e.EntityKey);
            foreach (var g in group)
            {
                var first = g.First(); //Maybe add support for setting tail, eg. (root)user -> addresses -> (root)user
                first.Populate(g); //Mutable... should be rewritten as non mutable. Can be done with help of DeepClone of first. But then the first needs to be tagged serializable.
                yield return first;
            }
        }
    }
}
