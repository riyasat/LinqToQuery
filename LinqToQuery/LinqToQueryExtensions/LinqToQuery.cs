using System.Data.SqlClient;
using System.Dynamic;
using System.Text;
using System.Threading;
using LinqToQueryExtensions.Delete;
using LinqToQueryExtensions.Insert;
using LinqToQueryExtensions.Select;
using LinqToQueryExtensions.Update;

namespace LinqToQueryExtensions
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
