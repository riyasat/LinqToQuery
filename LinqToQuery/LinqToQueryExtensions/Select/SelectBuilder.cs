//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using LinqToQueryExtensions.Base;
using LinqToQueryExtensions.Columns;
using LinqToQueryExtensions.Join;
using LinqToQueryExtensions.Order;
using LinqToQueryExtensions.Utilities.Enums;
using LinqToQueryExtensions.Where;

namespace LinqToQueryExtensions.Select
{
	public class SelectBuilder<TModel>
	{
		private readonly SqlConnection _connection;
		internal int _take { get; set; }
		internal int _page { get; set; }
		internal List<Column> SelectColumns;
		internal List<Column> JoinedColumns;
		internal List<Column> WhereColumns;
		internal List<Column> OrderByColumns;
		internal List<Column> AggregateColumns;
		internal List<Column> GroupByColumns;

		public SelectBuilder(SqlConnection connection)
		{
			_connection = connection;
			SelectColumns = new List<Column>();
			JoinedColumns = new List<Column>();
			WhereColumns = new List<Column>();
			OrderByColumns = new List<Column>();
			AggregateColumns = new List<Column>();
			GroupByColumns = new List<Column>();
		}
		public SelectBuilder<TModel> Columns(Expression<Action<BaseSelect<TModel>>> expression)
		{
			var res = new BaseSelect<TModel>();
			expression.Compile().Invoke(res);
			SelectColumns.AddRange(res.Columns);
			return this;
		}
		public SelectBuilder<TModel> Where(Expression<Action<WhereBuilder<TModel>>> expression)
		{
			var res = new WhereBuilder<TModel>();
			expression.Compile().Invoke(res);
			WhereColumns.AddRange(res.Conditions);
			return this;
		}
		public SelectBuilder<TModel> Join(Expression<Action<JoinBuilder<TModel>>> expression)
		{
			var res = new JoinBuilder<TModel>();
			expression.Compile().Invoke(res);
			JoinedColumns.AddRange(res.Joins);
			return this;
		}
		public SelectBuilder<TModel> OrderBy(Expression<Action<OrderByBuilder<TModel>>> expression)
		{
			var res = new OrderByBuilder<TModel>();
			expression.Compile().Invoke(res);
			OrderByColumns.AddRange(res.OrderAs);
			return this;
		}
		public SelectBuilder<TModel> Take(int take)
		{
			_take = take;
			return this;
		}
		public SelectBuilder<TModel> Page(int page)
		{
			_page = page;
			return this;
		}
		public string ToSqlServer2012Query()
		{
			if ((_take > 0 || _page > 0) && OrderByColumns.Any() == false)
			{
				throw new Exception("SQL Server requires order by in order to use pagination");
			}
			var select = BuildSelect() + BuildJoins() + BuildWhere() + BuildOrderBy() + BuildGroupBy();
			if (_page > 0)
			{
				select += $" OFFSET {(_page - 1) * _take} ROWS FETCH NEXT {_take} ROWS ONLY";
			}

			return select;
		}

		private string BuildGroupBy()
		{
			var groupBy = " GROUP BY ";
			var groupByColumns = "";
			if (SelectColumns.Any(x => x.AggregationMethod != AggregationMethods.NONE))
			{
				foreach (var selectedColumn in SelectColumns.Where(x => x.AggregationMethod == AggregationMethods.NONE))
				{
					if (groupByColumns.Length > 0)
					{
						groupByColumns += ",";
					}
					groupByColumns += $"{selectedColumn.TableAlias}.{selectedColumn.ColumnName}";
				}
			}
			if (groupByColumns.Length > 0)
			{
				return $" GROUP BY {groupByColumns}";
			}
			return null;
		}

		public IEnumerable<TModel> ToResult()
		{
			var query = ToSqlServer2012Query();
			var whereParams = GetWhereParams();
			return _connection.Query<TModel>(query, whereParams);
		}
		public IEnumerable<TType> ToResult<TType>()
		{
			var query = ToSqlServer2012Query();
			var whereParams = GetWhereParams();
			return _connection.Query<TType>(query, whereParams);
		}
		private DynamicParameters GetWhereParams()
		{
			var param = new DynamicParameters();
			foreach (var whereColumn in WhereColumns)
			{
				if (whereColumn.Condition == ConditionTypes.IN || whereColumn.Condition == ConditionTypes.NOTIN)
				{
					var valu = whereColumn.Value as string[];
					if (valu != null)
					{
						for (int i = 0; i < valu.Length; i++)
						{
							param.Add($"@{whereColumn.ColumnName}{i}", valu[i]);
						}
					}
				}
				else
				{
					param.Add($@"{whereColumn.ColumnName}", whereColumn.Value);
				}
			}
			return param;

		}

		public string ToSqlServer2008Query()
		{
			if ((_take > 0 || _page > 0) && OrderByColumns.Any() == false)
			{
				throw new Exception("SQL Server requires order by in order to use pagination");
			}
			var select = BuildSelect() + BuildJoins() + BuildWhere() + BuildOrderBy();
			if (_page > 0)
			{
				if (_take <= 0) throw new Exception("Take cannot be zero when using pagination");

				select += $" OFFSET {(_page - 1) * _take} ROWS FETCH NEXT {_take} ROWS ONLY";
			}

			return select;
		}
		public string ToMySqlQuery()
		{
			if ((_take > 0 || _page > 0) && OrderByColumns.Any() == false)
			{
				throw new Exception("MY SQL requires order by in order to use pagination");
			}
			var select = BuildSelect() + BuildJoins() + BuildWhere() + BuildOrderBy();
			if (_page > 0)
			{
				select += $" LIMIT {_take}, {(_page - 1) * _take} ";
			}

			return select;
		}
		internal string BuildSelect()
		{
			var query = "";
			var mainTableAlias = typeof(TModel).Name;
			if (SelectColumns.Any() == false) throw new Exception("There are no columns to Select, Please check your linq");
			var tableAttribute = typeof(TModel).GetCustomAttribute<TableAttribute>();

			foreach (var selectColumn in SelectColumns)
			{
				if (string.IsNullOrWhiteSpace(query) == false) query += ",";

				query += selectColumn.GetSelect();
			}
			if (tableAttribute != null)
			{
				var tableName = string.IsNullOrWhiteSpace(tableAttribute.Schema)
					? $"{tableAttribute.Name} AS {mainTableAlias}"
					: $"{tableAttribute.Schema}.{tableAttribute.Name} AS {mainTableAlias}";

				query += $" FROM {tableName}";
			}
			else
			{
				query += $" FROM {mainTableAlias} AS {mainTableAlias} ";
			}


			return $"SELECT {query}";
		}

		internal string BuildOrderBy()
		{
			string query = "";
			if (OrderByColumns.Any())
			{
				var orderBy = "";
				foreach (var orderByColumn in OrderByColumns)
				{
					if (string.IsNullOrWhiteSpace(orderBy) == false) orderBy += ",";
					orderBy += orderByColumn.GetOrderBy();
				}
				if (string.IsNullOrWhiteSpace(orderBy) == false)
				{
					query += $" ORDER BY {orderBy}";
				}
			}
			return query;
		}

		internal string BuildWhere()
		{
			string query = "";
			if (WhereColumns.Any())
			{
				query += " WHERE ";
				foreach (var whereColumn in WhereColumns)
				{
					query += whereColumn.GetWhere();
				}
			}
			return query;
		}

		internal string BuildJoins()
		{
			var mainTableAlias = typeof(TModel).Name;
			if (SelectColumns.Any() == false) throw new Exception("There are no columns to Select, Please check your linq");
			string query = "";
			var countMatchingAlias = 0;
			foreach (var joinedColumn in JoinedColumns)
			{
				if (joinedColumn.TableAlias == mainTableAlias)
				{
					joinedColumn.TableAlias = mainTableAlias + (countMatchingAlias++);
				}
				query += joinedColumn.GetJoin();
			}
			return query;
		}
	}
}