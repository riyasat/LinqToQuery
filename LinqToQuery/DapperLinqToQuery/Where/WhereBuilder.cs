//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using DapperLinqToQuery.Base;
using DapperLinqToQuery.Columns;
using DapperLinqToQuery.Utilities;

namespace DapperLinqToQuery.Where
{
	public class WhereBuilder<TModel>
	{
		internal IList<Column> Conditions = new List<Column>();
		public BaseWhere<TModel> Column(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateWhereCondition<TModel>(colName);
			Conditions.Add(column);
			return new BaseWhere<TModel>(this);
		}
		public BaseWhere<TModel> Column<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateWhereCondition<TJoin>(colName);
			Conditions.Add(column);
			return new BaseWhere<TModel>(this);
		}

		private Column GenerateWhereCondition<TJoin>(string colName)
		{
			var columnAttribute = ExpressionExtensions.GetAttributeValue<TJoin, ColumnAttribute>(colName) as ColumnAttribute;
			var tableAttribute = typeof(TJoin).GetCustomAttribute<TableAttribute>();

			var column = new Column(colName)
			{
				TableAlias = typeof(TJoin).Name,
				TableName = typeof(TJoin).Name
			};

			if (tableAttribute != null)
			{
				column.TableName = string.IsNullOrWhiteSpace(tableAttribute.Schema)
					? tableAttribute.Name
					: $"{tableAttribute.Schema}.{tableAttribute.Name}";
			}
			if (columnAttribute != null)
			{
				column.ColumnName = columnAttribute.Name;
				column.ColumnAlias = colName;
			}
			return column;
		}
	}
}
