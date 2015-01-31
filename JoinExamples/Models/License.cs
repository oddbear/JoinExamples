using JoinExamples.Models;
using JoinExamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public partial class License : Entity
    {
        internal override object EntityKey { get { return new { Id }; } }

        internal override void Populate<T>(IEnumerable<T> collection)
        {
            var col = collection.Cast<License>();
            this.Address = col.NormalizeToSingle(c => c.Address);
        }

        internal License Map(License tmp)
        {
            this.Address = tmp.Address;
            return this;
        }
    }
}
