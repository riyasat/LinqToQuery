//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryCore.Base;
using LinqToQueryCore.Columns;
using LinqToQueryCore.Utilities;
using LinqToQueryCore.Utilities.Enums;

namespace LinqToQueryCore.Join
{
	public class JoinBuilder<TModel>
	{
		internal IList<Column> Joins;

		public JoinBuilder()
		{
			Joins = new List<Column>();
		}

		//public BaseJoin<TModel> Column(Expression<Func<TModel, object>> expression, JoinTypes jointType)
		//{
		//	var colName = expression.GetMemberInfo();
		//	var column = GenerateJoin<TModel>(colName, jointType);
		//	Joins.Add(column);
		//	return new BaseJoin<TModel>(this);
		//}

		public BaseJoin<TModel> Column<TJoin>(Expression<Func<TJoin, object>> expression, JoinTypes jointType)
		{
			var colName = expression.GetMemberInfo();
			var column = GenerateJoin<TJoin>(colName, jointType);
			Joins.Add(column);
			return new BaseJoin<TModel>(this);
		}

		private Column GenerateJoin<TJoin>(string colName, JoinTypes jointType)
		{
			var columnAttribute = ExpressionExtensions.GetAttributeValue<TJoin, ColumnAttribute>(colName) as ColumnAttribute;
			var tableAlias = typeof(TJoin).Name;
			var tableAttribute = typeof(TJoin).GetCustomAttribute<TableAttribute>();

			var countSameAlias = Joins.Count(x => x.TableAlias == tableAlias);

			var column = new Column(colName)
			{
				JoinType = jointType,
				TableAlias = countSameAlias > 1 ? tableAlias + (countSameAlias - 1) : tableAlias,
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