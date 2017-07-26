//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryExtensions.Columns;
using LinqToQueryExtensions.Utilities;

namespace LinqToQueryExtensions.Base
{
	public class BaseUpdate<TModel>
	{
		internal IList<Column> Columns;

		public BaseUpdate()
		{
			Columns = new List<Column>();
		}

		public BaseUpdate<TModel> Set<TType>(Expression<Func<TModel, TType>> expression, TType value)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateSelectColumn<TModel>(colName);
			column.Value = value;
			Columns.Add(column);
			return this;
		}

		private Column GenerateSelectColumn<TJoin>(string colName)
		{
			var columnAttribute = ExpressionExtensions.GetAttributeValue<TJoin, ColumnAttribute>(colName) as ColumnAttribute;
			var tableAttribute = typeof(TJoin).GetCustomAttribute<TableAttribute>();

			var column = new Column(colName)
			{
				TableAlias = typeof(TJoin).Name,
				TableName = typeof(TJoin).Name
			};

			if (columnAttribute != null)
			{
				column.ColumnName = columnAttribute.Name;
				column.ColumnAlias = colName != "*" ? colName : "";
			}

			if (tableAttribute != null)
			{
				column.TableName = string.IsNullOrWhiteSpace(tableAttribute.Schema)
					? tableAttribute.Name
					: $"{tableAttribute.Name}.{tableAttribute.Name}";
			}
			return column;
		}
	}
}