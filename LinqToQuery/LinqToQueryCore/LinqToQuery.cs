﻿//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System.Data.SqlClient;
using LinqToQueryCore.Delete;
using LinqToQueryCore.Insert;
using LinqToQueryCore.Select;
using LinqToQueryCore.Update;

namespace LinqToQueryCore
{
	public static class LinqToQuery
	{
		public static SelectBuilder<TModel> Select<TModel>(this SqlConnection connection)
		{
			return new SelectBuilder<TModel>(connection);
		}
		public static InsertBuilder Insert(this SqlConnection connection)
		{
			return new InsertBuilder(connection);
		}

		public static DeleteBuilder<TModel> Delete<TModel>(this SqlConnection connection)
		{
			return new DeleteBuilder<TModel>(connection);
		}
		public static UpdateBuilder<TModel> Update<TModel>(this SqlConnection connection)
		{
			return new UpdateBuilder<TModel>(connection);
		}
	}
}
