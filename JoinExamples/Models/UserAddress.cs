using JoinExamples.Models;
using JoinExamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public partial class UserAddress : Entity
    {
        internal override object EntityKey { get { return new { this.AddressId, this.UserId }; } }

        internal override void Populate<T>(IEnumerable<T> collection)
        {
            var col = collection.Cast<UserAddress>();
            this.User = col.NormalizeToSingle(c => c.User);
            this.Address = col.NormalizeToSingle(c => c.Address);
        }

        internal UserAddress Map(UserAddress tmp)
        {
            this.User = tmp.User;
            this.Address = tmp.Address;
            return this;
        }
    }
}
