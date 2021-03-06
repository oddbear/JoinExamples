//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : TestBase
	/// Data Source    : .
	/// Server Version : 12.00.2000
	/// </summary>
	public partial class TestBaseDB : LinqToDB.Data.DataConnection
	{
		public ITable<Address>     Addresses     { get { return this.GetTable<Address>(); } }
		public ITable<License>     Licenses      { get { return this.GetTable<License>(); } }
		public ITable<Street>      Streets       { get { return this.GetTable<Street>(); } }
		public ITable<User>        Users         { get { return this.GetTable<User>(); } }
		public ITable<UserAddress> UserAddresses { get { return this.GetTable<UserAddress>(); } }

		public TestBaseDB()
		{
			InitDataContext();
		}

		public TestBaseDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();

		#region FreeTextTable

		public class FreeTextKey<T>
		{
			public T   Key;
			public int Rank;
		}

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable,TKey>(string field, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),
				field,
				text);
		}

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable,TKey>(Expression<Func<TTable,string>> fieldSelector, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				((MethodInfo)(MethodBase.GetCurrentMethod())).MakeGenericMethod(typeof(TTable), typeof(TKey)),
				fieldSelector,
				text);
		}

		#endregion
	}

	[Table(Schema="dbo", Name="Address")]
	public partial class Address
	{
		[PrimaryKey, Identity   ] public int   Id          { get; set; } // int
		[Column,     NotNull    ] public int   StreetId    { get; set; } // int
		[Column,     NotNull    ] public int   HouseNumber { get; set; } // int
		[Column,        Nullable] public char? HouseLetter { get; set; } // char(1)

		#region Associations

		/// <summary>
		/// FK_Address_Street
		/// </summary>
		[Association(ThisKey="StreetId", OtherKey="Id", CanBeNull=false)]
		public Street Street { get; set; }

		/// <summary>
		/// FK_UserAddress_Address_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AddressId", CanBeNull=true)]
		public IEnumerable<UserAddress> UserAddresses { get; set; }

		/// <summary>
		/// FK_License_Address_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AddressId", CanBeNull=true)]
		public IEnumerable<License> Licenses { get; set; }

		/// <summary>
		/// FK_User_Address_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="PrimaryAddressId", CanBeNull=true)]
		public IEnumerable<User> Users { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="License")]
	public partial class License
	{
		[PrimaryKey, Identity] public int    Id                 { get; set; } // int
		[Column,     NotNull ] public int    AddressId          { get; set; } // int
		[Column,     NotNull ] public string LicenseDescription { get; set; } // nvarchar(50)

		#region Associations

		/// <summary>
		/// FK_License_Address
		/// </summary>
		[Association(ThisKey="AddressId", OtherKey="Id", CanBeNull=false)]
		public Address Address { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="Street")]
	public partial class Street
	{
		[PrimaryKey, Identity] public int    Id   { get; set; } // int
		[Column,     NotNull ] public string Name { get; set; } // nvarchar(50)

		#region Associations

		/// <summary>
		/// FK_Address_Street_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StreetId", CanBeNull=true)]
		public IEnumerable<Address> Addresses { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="User")]
	public partial class User
	{
		[PrimaryKey, Identity   ] public int    Id               { get; set; } // int
		[Column,     NotNull    ] public string Name             { get; set; } // nvarchar(50)
		[Column,        Nullable] public int?   PrimaryAddressId { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_User_Address
		/// </summary>
		[Association(ThisKey="PrimaryAddressId", OtherKey="Id", CanBeNull=true)]
		public Address Address { get; set; }

		/// <summary>
		/// FK_UserAddress_User_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true)]
		public IEnumerable<UserAddress> UserAddresses { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="UserAddress")]
	public partial class UserAddress
	{
		[PrimaryKey(1), NotNull] public int UserId    { get; set; } // int
		[PrimaryKey(2), NotNull] public int AddressId { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_UserAddress_Address
		/// </summary>
		[Association(ThisKey="AddressId", OtherKey="Id", CanBeNull=false)]
		public Address Address { get; set; }

		/// <summary>
		/// FK_UserAddress_User
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=false)]
		public User User { get; set; }

		#endregion
	}

	public static partial class TestBaseDBStoredProcedures
	{
		#region sp_helpdiagrams

		public partial class sp_helpdiagramsResult
		{
			public string Database { get; set; }
			public string Name     { get; set; }
			public int    ID       { get; set; }
			public string Owner    { get; set; }
			public int    OwnerID  { get; set; }
		}

		public static IEnumerable<sp_helpdiagramsResult> sp_helpdiagrams(this DataConnection dataConnection, string @diagramname, int? @owner_id)
		{
			return dataConnection.QueryProc<sp_helpdiagramsResult>("[dbo].[sp_helpdiagrams]",
				new DataParameter("@diagramname", @diagramname),
				new DataParameter("@owner_id",    @owner_id));
		}

		#endregion

		#region sp_helpdiagramdefinition

		public partial class sp_helpdiagramdefinitionResult
		{
			public int?   version    { get; set; }
			public byte[] definition { get; set; }
		}

		public static IEnumerable<sp_helpdiagramdefinitionResult> sp_helpdiagramdefinition(this DataConnection dataConnection, string @diagramname, int? @owner_id)
		{
			return dataConnection.QueryProc<sp_helpdiagramdefinitionResult>("[dbo].[sp_helpdiagramdefinition]",
				new DataParameter("@diagramname", @diagramname),
				new DataParameter("@owner_id",    @owner_id));
		}

		#endregion

		#region sp_creatediagram

		public static int sp_creatediagram(this DataConnection dataConnection, string @diagramname, int? @owner_id, int? @version, byte[] @definition)
		{
			return dataConnection.ExecuteProc("[dbo].[sp_creatediagram]",
				new DataParameter("@diagramname", @diagramname),
				new DataParameter("@owner_id",    @owner_id),
				new DataParameter("@version",     @version),
				new DataParameter("@definition",  @definition));
		}

		#endregion

		#region sp_renamediagram

		public static int sp_renamediagram(this DataConnection dataConnection, string @diagramname, int? @owner_id, string @new_diagramname)
		{
			return dataConnection.ExecuteProc("[dbo].[sp_renamediagram]",
				new DataParameter("@diagramname",     @diagramname),
				new DataParameter("@owner_id",        @owner_id),
				new DataParameter("@new_diagramname", @new_diagramname));
		}

		#endregion

		#region sp_alterdiagram

		public static int sp_alterdiagram(this DataConnection dataConnection, string @diagramname, int? @owner_id, int? @version, byte[] @definition)
		{
			return dataConnection.ExecuteProc("[dbo].[sp_alterdiagram]",
				new DataParameter("@diagramname", @diagramname),
				new DataParameter("@owner_id",    @owner_id),
				new DataParameter("@version",     @version),
				new DataParameter("@definition",  @definition));
		}

		#endregion

		#region sp_dropdiagram

		public static int sp_dropdiagram(this DataConnection dataConnection, string @diagramname, int? @owner_id)
		{
			return dataConnection.ExecuteProc("[dbo].[sp_dropdiagram]",
				new DataParameter("@diagramname", @diagramname),
				new DataParameter("@owner_id",    @owner_id));
		}

		#endregion
	}

	public static partial class SqlFunctions
	{
		#region fn_diagramobjects

		[Sql.Function(Name="fn_diagramobjects", ServerSideOnly=true)]
		public static int? fn_diagramobjects()
		{
			throw new InvalidOperationException();
		}

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Address Find(this ITable<Address> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static License Find(this ITable<License> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Street Find(this ITable<Street> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static User Find(this ITable<User> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UserAddress Find(this ITable<UserAddress> table, int UserId, int AddressId)
		{
			return table.FirstOrDefault(t =>
				t.UserId    == UserId &&
				t.AddressId == AddressId);
		}
	}
}
