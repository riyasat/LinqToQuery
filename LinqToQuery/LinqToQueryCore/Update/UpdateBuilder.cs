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
using LinqToQueryCore.Base;
using LinqToQueryCore.Columns;
using LinqToQueryCore.Join;
using LinqToQueryCore.Utilities.Enums;
using LinqToQueryCore.Where;

namespace LinqToQueryCore.Update
{
	public class UpdateBuilder<TModel>
	{
		private readonly SqlConnection _connection;
		internal List<Column> UpdateColumns;
		internal List<Column> JoinedColumns;
		internal List<Column> WhereColumns;
		public UpdateBuilder(SqlConnection connection)
		{
			_connection = connection;
			UpdateColumns = new List<Column>();
			JoinedColumns = new List<Column>();
			WhereColumns = new List<Column>();
		}
		public UpdateBuilder<TModel> Columns(Expression<Action<BaseUpdate<TModel>>> expression)
		{
			var res = new BaseUpdate<TModel>();
			expression.Compile().Invoke(res);
			UpdateColumns.AddRange(res.Columns);
			return this;
		}
		public UpdateBuilder<TModel> Where(Expression<Action<WhereBuilder<TModel>>> expression)
		{
			var res = new WhereBuilder<TModel>();
			expression.Compile().Invoke(res);
			WhereColumns.AddRange(res.Conditions);
			return this;
		}
		public UpdateBuilder<TModel> Join(Expression<Action<JoinBuilder<TModel>>> expression)
		{
			var res = new JoinBuilder<TModel>();
			expression.Compile().Invoke(res);
			JoinedColumns.AddRange(res.Joins);
			return this;
		}
		private DynamicParameters GetParams()
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

			foreach (var updateColumn in UpdateColumns)
			{
				param.Add($"@UP{updateColumn.ColumnName}", updateColumn.Value);

			}
			return param;

		}
		private string BuildUpdate()
		{
			var query = "";
			var columnSet = "";
			var mainTableAlias = typeof(TModel).Name;
			if (UpdateColumns.Any() == false) throw new Exception("There are no columns to Update, Please check your linq");
			var tableAttribute = typeof(TModel).GetCustomAttribute<TableAttribute>();

			columnSet = String.Join(",", UpdateColumns.Select(x => x.GetSetValue()));

			if (tableAttribute != null)
			{
				var tableName = string.IsNullOrWhiteSpace(tableAttribute.Schema)
					? $"{tableAttribute.Name} AS {mainTableAlias}"
					: $"{tableAttribute.Schema}.{tableAttribute.Name} AS {mainTableAlias}";

				query += $"UPDATE {mainTableAlias} SET {columnSet} FROM {tableName}";
			}
			else
			{
				query += $"UPDATE {mainTableAlias} SET {columnSet} FROM {mainTableAlias} AS {mainTableAlias}";
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
		public string ToQuery()
		{
			return BuildUpdate();
		}

		public void Execute(IsolationLevel transactionLevel)
		{
			var query = ToQuery();
			if(_connection.State == ConnectionState.Closed)
				_connection.Open();
			using (_connection)
			{
				using (var tr = _connection.BeginTransaction(transactionLevel))
				{
					try
					{
						_connection.Execute(query, GetParams(), tr);
						tr.Commit();
					}
					catch (Exception e)
					{
						tr.Rollback();						
						throw;
					}
				}
			}
		}
	}
}