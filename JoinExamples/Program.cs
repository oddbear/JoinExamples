using DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoinExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new TestBaseDB())
            {
                //Could couse StackOverflow Exceptions if to complex, fixed in Linq2Db from #157:
                var q = db.Users
                    .LeftJoin(db.Addresses, u => u.PrimaryAddressId, a => a.Id, u => u.Address)
                    .Join(db.Streets, u => u.Address.StreetId, s => s.Id, u => u.Address.Street)
                    .LeftJoin(db.Licenses, u => u.Address.Id, l => l.AddressId, u => u.Address.Ø_Licenses)
                    //.LeftJoin(db.UserAddresses, u => u.Id, ua => ua.UserId, u => u.Ø_UserAddresses)
                    //.LeftJoin(db.Addresses, u => u.Ø_UserAddresses.AddressId, a => a.Id, u => u.Ø_UserAddresses.Address)
                    //.Join(db.Streets, u => u.Ø_UserAddresses.Address.StreetId, s => s.Id, u => u.Ø_UserAddresses.Address.Street)
                    //.LeftJoin(db.Licenses, u => u.Ø_UserAddresses.Address.Id, l => l.AddressId, u => u.Ø_UserAddresses.Address.Ø_Licenses)

                    //.Select(u => new User { Id = u.Id }) //This will work, and generate beautiful SQL.
                ;

                var sw = Stopwatch.StartNew();
                var r = q.AsNormalized(); //Will make the "flat" properties to collections(if logic is provided), everything from this point will be in memory context.
                sw.Stop();
                Console.WriteLine(sw.Elapsed);

                //If you don't use AsNormalized or similar, the row count could be so high that JSON.Net gets a OutOfMemoryException.
                System.IO.File.WriteAllText(@"c:\temp\output.json", JsonConvert.SerializeObject(r, Formatting.Indented));
            }
        }
    }
}
