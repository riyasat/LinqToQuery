//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryExtensions.Base;
using LinqToQueryExtensions.Columns;
using LinqToQueryExtensions.Utilities;
using LinqToQueryExtensions.Utilities.Enums;

namespace LinqToQueryExtensions.Order
{
	public class OrderByBuilder<TModel>
	{
		internal IList<Column> OrderAs;

		public OrderByBuilder()
		{
			OrderAs = new List<Column>();
		}
		
		public BaseOrderBy<TModel> Column(Expression<Func<TModel, object>> expression, OrderByDirection orderByDirection)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateOrderBy<TModel>(colName, orderByDirection);
			OrderAs.Add(column);
			
			return new BaseOrderBy<TModel>(this);
		}

		public BaseOrderBy<TModel> Column<TJoin>(Expression<Func<TJoin, object>> expression, OrderByDirection orderByDirection)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateOrderBy<TJoin>(colName, orderByDirection);
			OrderAs.Add(column);
			return new BaseOrderBy<TModel>(this);
		}

		internal Column GenerateOrderBy<TJoin>(string colName, OrderByDirection orderByDirection)
		{

			var columnAttribute = ExpressionExtensions.GetAttributeValue<TJoin, ColumnAttribute>(colName) as ColumnAttribute;
			var tableAttribute = typeof(TJoin).GetCustomAttribute<TableAttribute>();

			var column = new Column(colName)
			{
				OrderBy = orderByDirection,
				TableAlias = typeof(TJoin).Name,
				TableName = typeof(TJoin).Name
			};

			if (columnAttribute != null)
			{
				column.ColumnName = columnAttribute.Name;
				column.ColumnAlias = colName;
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