using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToQueryExample.Entities;
using LinqToQueryExtensions;
using LinqToQueryExtensions.Utilities.Enums;

namespace LinqToQueryExample
{
	class Program
	{
		static void Main(string[] args)
		{
			var conn = new SqlConnection("Server=[YourServerName];Database=SampleDatabase;Trusted_Connection=True;");
			//conn.Insert().Entity(new Supplier
			//	{
			//		City = "Test",
			//		CompanyName = "A company name"
			//	}).Execute(IsolationLevel.ReadCommitted)
			//		;

			var result = conn.Select<Supplier>()
				.Columns(c=>c.Sum(x=>x.Id).Add(x=>x.CompanyName))
				.ToResult<int>().First();

			Console.WriteLine(result);
			Console.ReadKey();

		}
	}
}
