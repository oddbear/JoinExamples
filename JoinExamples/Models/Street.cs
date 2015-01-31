using JoinExamples.Models;
using JoinExamples;
using System.Collections.Generic;
using System.Linq;

namespace DataModels
{
    public partial class Street : Entity
    {
        internal override object EntityKey { get { return new { Id }; } }
        internal DataModels.Address Ø_Addresses
        {
            get { return GetValue(Addresses); }
            set { Addresses = SetValue(value); }
        }

        internal override void Populate<T>(IEnumerable<T> collection)
        {
            var col = collection.Cast<Street>();
            this.Addresses = col.Normalize(s => s.Ø_Addresses);
        }

        internal Street Map(Street tmp)
        {
            this.Ø_Addresses = tmp.Ø_Addresses;
            return this;
        }
    }
}