//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryExtensions.Columns;
using LinqToQueryExtensions.Utilities;
using LinqToQueryExtensions.Utilities.Attributes;

namespace LinqToQueryExtensions.Base
{
	public class BaseSelect<TModel>
	{
		internal IList<Column> Columns;

		public BaseSelect()
		{
			Columns = new List<Column>();
		}

		public BaseSelect<TModel> All()
		{
			var column = GenerateSelectColumn<TModel>("*");
			Columns.Add(column);
			return this;
		}
		public BaseSelect<TModel> All<TJoin>()
		{
			var column = GenerateSelectColumn<TJoin>("*");
			Columns.Add(column);
			return this;
		}
		public BaseSelect<TModel> Add(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateSelectColumn<TModel>(colName);
			Columns.Add(column);
			return this;
		}
		public BaseSelect<TModel> Add<TJoin, TMap>(Expression<Func<TJoin, object>> expression, Expression<Func<TMap, object>> expressionToMap)
		{
			var colName = expression.GetMemberInfo();
			var colAlias = expressionToMap.GetMemberInfo();
			var column = GenerateSelectColumn<TJoin>(colName);
			column.ColumnAlias = colAlias;
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