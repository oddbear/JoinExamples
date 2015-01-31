using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoinExamples.Models
{
    public abstract class Entity
    {
        internal abstract object EntityKey { get; }
        internal abstract void Populate<T>(IEnumerable<T> collection);

        protected T GetValue<T>(IEnumerable<T> value)
            where T : class
        {
            if (value == null || value.Count() != 1)
                return null;
            return value.Single();
        }
        protected IEnumerable<T> SetValue<T>(T value)
            where T : class
        {
            if (value == null)
                return Enumerable.Empty<T>();
            return new[] { value };
        }
    }
}
