using JoinExamples.Models;
using JoinExamples;
using System.Collections.Generic;
using System.Linq;

namespace DataModels
{
    //You know the primary key by the PrimaryKey Attribute.
    //You know Accosiations from attribute, and Inner T.
    //If in this namespace, or have som attribute, enharitance or whatever. Autofix at Normalization.

    //This mappingcode can be automated written with changes in T4:
    public partial class User : Entity
    {
        internal override object EntityKey { get { return new { Id }; } }
        internal UserAddress Ø_UserAddresses
        {
            get { return GetValue(UserAddresses); }
            set { UserAddresses = SetValue(value); }
        }

        internal override void Populate<T>(IEnumerable<T> collection) //Alle Users i samme gruppe.
        {
            var col = collection.Cast<User>();
            this.Address = col.NormalizeToSingle(c => c.Address);
            this.UserAddresses = col.Normalize(s => s.Ø_UserAddresses);
        }

        internal User Map(User tmp)
        {
            this.Address = tmp.Address;
            this.Ø_UserAddresses = tmp.Ø_UserAddresses;
            return this;
        }
    }
}
