//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using DapperLinqToQuery.Columns;
using DapperLinqToQuery.Join;
using DapperLinqToQuery.Utilities.Enums;
using DapperLinqToQuery.Where;

namespace DapperLinqToQuery.Delete
{
	public class DeleteBuilder<TModel>
	{
		private readonly SqlConnection _connection;
		internal List<Column> JoinedColumns;
		internal List<Column> WhereColumns;
		public DeleteBuilder(SqlConnection connection)
		{
			_connection = connection;
			JoinedColumns = new List<Column>();
			WhereColumns = new List<Column>();
		}

		public DeleteBuilder<TModel> Where(Expression<Action<WhereBuilder<TModel>>> expression)
		{
			var res = new WhereBuilder<TModel>();
			expression.Compile().Invoke(res);
			WhereColumns.AddRange(res.Conditions);
			return this;
		}

		public DeleteBuilder<TModel> Join(Expression<Action<JoinBuilder<TModel>>> expression)
		{
			var res = new JoinBuilder<TModel>();
			expression.Compile().Invoke(res);
			JoinedColumns.AddRange(res.Joins);
			return this;
		}

		public string ToSqlServerQuery()
		{
			return BuildQuery();
		}

		public void ExecuteSqlServerQuery(IsolationLevel transactionLevel)
		{

			var query = ToSqlServerQuery();
			var whereParams = GetWhereParams();
			if(_connection.State == ConnectionState.Closed)
				_connection.Open();
			using (var tr = _connection.BeginTransaction())
			{
				try
				{
					_connection.Execute(query, whereParams, tr);
					tr.Commit();
				}
				catch (Exception ex)
				{
					tr.Rollback();
					throw ex;
				}
			}
			
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
		private string BuildQuery()
		{
			var query = "";
			var mainTableAlias = typeof(TModel).Name;
			var tableAttribute = typeof(TModel).GetCustomAttribute<TableAttribute>();

			
			if (tableAttribute != null)
			{
				var tableName = string.IsNullOrWhiteSpace(tableAttribute.Schema)
					? $"{tableAttribute.Name} AS {mainTableAlias}"
					: $"{tableAttribute.Schema}.{tableAttribute.Name} AS {mainTableAlias}";

				query += $"DELETE {mainTableAlias} FROM {tableName}";
			}
			else
			{
				query += $"DELETE {mainTableAlias} FROM {mainTableAlias} AS {mainTableAlias} ";
			}
			var countMatchingAlias = 0;
			foreach (var joinedColumn in JoinedColumns)
			{
				if (joinedColumn.TableAlias == mainTableAlias)
				{
					joinedColumn.TableAlias = mainTableAlias + (countMatchingAlias++);
				}
				query += joinedColumn.GetJoin();
			}
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
	}
}