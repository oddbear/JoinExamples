# JoinExamples
Examples using flat entities and expression trees to make cleaner Joins.
There is also a Normalization method to make the the flat (1:1 row/Ø mapping) to a object tree representation.

What this example does, is changing this:
```
var q = db.Users
  .SelectMany(u => db.Addresses.Where(a => u.PrimaryAddressId == a.Id).DefaultIfEmpty(), (u, a) => new User() { Id = u.Id, PrimaryAddressId = u.PrimaryAddressId, Address = a })
  .SelectMany(u => db.Streets.Where(s => u.Address.StreetId == s.Id).DefaultIfEmpty(), (u, s) => new User() { Id = u.Id, PrimaryAddressId = u.PrimaryAddressId, Address = new Address() { Id = u.Address.Id, StreetId = u.Address.StreetId, HouseNumber = u.Address.HouseNumber, HouseLetter = u.Address.HouseLetter, Street = s } })
  .SelectMany(u => db.Licenses.Where(l => u.Address.Id == l.AddressId).DefaultIfEmpty(), (u, l) => new User() { Id = u.Id, PrimaryAddressId = u.PrimaryAddressId, Address = new Address() { Id = u.Address.Id, StreetId = u.Address.StreetId, HouseNumber = u.Address.HouseNumber, HouseLetter = u.Address.HouseLetter, Street = u.Address.Street, Ø_Licenses = l } })
;
```
Into this:
```
var q = db.Users
  .LeftJoin(db.Addresses, u => u.PrimaryAddressId, a => a.Id, u => u.Address)
  .Join(db.Streets, u => u.Address.StreetId, s => s.Id, u => u.Address.Street)
  .LeftJoin(db.Licenses, u => u.Address.Id, l => l.AddressId, u => u.Address.Ø_Licenses)
;
```

Things I might add in the future:
- Caching of Expression trees for speed.
- Expression trees in in Normalization to reduse plumbing code (only the flat representation should be necessary).
- Remove the "complex" mapping, making something similar to LINQ Querysyntax in LINQ2SQL.
- Something like a .Include() keyword.
