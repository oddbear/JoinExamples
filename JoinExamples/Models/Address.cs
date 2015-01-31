using JoinExamples.Models;
using JoinExamples;
using System.Collections.Generic;
using System.Linq;

namespace DataModels
{
    public partial class Address : Entity
    {
        internal override object EntityKey { get { return new { Id }; } }

        internal UserAddress Ø_UserAddresses
        {
            get { return GetValue(UserAddresses); }
            set { UserAddresses = SetValue(value); }
        }

        internal License Ø_Licenses
        {
            get { return GetValue(Licenses); }
            set { Licenses = SetValue(value); }
        }

        internal DataModels.User Ø_Users
        {
            get { return GetValue(Users); }
            set { Users = SetValue(value); }
        }

        internal override void Populate<T>(IEnumerable<T> collection)
        {
            var col = collection.Cast<Address>();
            this.Street = col.NormalizeToSingle(a => a.Street);
            this.UserAddresses = col.Normalize(a => a.Ø_UserAddresses);
            this.Licenses = col.Normalize(a => a.Ø_Licenses);
            this.Users = col.Normalize(a => a.Ø_Users);
        }

        internal Address Map(Address tmp)
        {
            this.Street = tmp.Street;
            this.Ø_UserAddresses = tmp.Ø_UserAddresses;
            this.Ø_Licenses = tmp.Ø_Licenses;
            this.Ø_Users = tmp.Ø_Users;
            return this;
        }
    }
}