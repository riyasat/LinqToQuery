//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryCore.Columns;
using LinqToQueryCore.Join;
using LinqToQueryCore.Utilities;

namespace LinqToQueryCore.Base
{
	public class BaseJoin<TModel>
	{
		private readonly JoinBuilder<TModel> _joinBuilder;

		public BaseJoin(JoinBuilder<TModel> joinBuilder)
		{
			_joinBuilder = joinBuilder;
		}

		public JoinBuilder<TModel> With<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var column = _joinBuilder.Joins.Last();
			var joinedColumn = GenerateJoinedColumn<TJoin>(colName);
			column.JoinWithColumn = joinedColumn;
			return _joinBuilder;
		}
		public JoinBuilder<TModel> With(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var column = _joinBuilder.Joins.Last();
			var joinedColumn = GenerateJoinedColumn<TModel>(colName);
			column.JoinWithColumn = joinedColumn;
			return _joinBuilder;
		}
		private Column GenerateJoinedColumn<TJoin>(string colName)
		{
			var columnAttribute = ExpressionExtensions.GetAttributeValue<TJoin, ColumnAttribute>(colName) as ColumnAttribute;
			var tableAlias = typeof(TJoin).Name;
			var tableAttribute = typeof(TJoin).GetCustomAttribute<TableAttribute>();
			var column = new Column(colName)
			{
				TableAlias = tableAlias,
				TableName = tableAlias
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