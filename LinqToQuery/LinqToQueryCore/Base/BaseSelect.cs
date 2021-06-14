//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToQueryCore.Columns;
using LinqToQueryCore.Utilities;
using LinqToQueryCore.Utilities.Enums;

namespace LinqToQueryCore.Base
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
		public BaseSelect<TModel> Add<TJoin>(Expression<Func<TJoin, object>> expression, Expression<Func<TModel, object>> expressionToMap)
		{
			var colName = expression.GetMemberInfo();
			var colAlias = expressionToMap.GetMemberInfo();
			var column = GenerateSelectColumn<TJoin>(colName);
			column.ColumnAlias = colAlias;
			Columns.Add(column);
			return this;
		}
		public BaseSelect<TModel> Add<TJoin>(Expression<Func<TJoin, object>> expression, string columnAlias)
		{
			var colName = expression.GetMemberInfo();
			var colAlias = columnAlias;
			var column = GenerateSelectColumn<TJoin>(colName);
			column.ColumnAlias = colAlias;
			Columns.Add(column);
			return this;
		}

		#region Aggregate Functions

		private bool isValidNumericValue(object value)
		{
			if (value == null) 
				throw new Exception("Aggregated method can only work with numeric data");

			switch (value.ToString().ToLower())
			{
				case "int":
				case "decimel":
				case "float":
				case "int64":
				case "int32":
					return true;

				default:
					throw new Exception("Aggregated method can only work with numeric data");
			}
			

		}

		public BaseSelect<TModel> DistinctBy<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			if (Columns.Any(x => x.HasDistinctBy))
			{
				throw new Exception("There is already a distinct by column added to the query. You can only add one distinct by Column");
			}
			var column = GenerateSelectColumn<TJoin>(colName);
			column.DistinctBy = colName;
			Columns.Add(column);
			return this;
		}

		public BaseSelect<TModel> DistinctBy(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			if (Columns.Any(x => x.HasDistinctBy))
			{
				throw new Exception("There is already a distinct by column added to the query. You can only add one distinct by Column");
			}
			var column = GenerateSelectColumn<TModel>(colName);
			column.DistinctBy = colName;
			Columns.Add(column);
			return this;
		}

		public BaseSelect<TModel> Sum<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TJoin>(colName);
				column.AggregationMethod = AggregationMethods.SUM;
				Columns.Add(column);
			}
			return this;
		}


		public BaseSelect<TModel> Sum(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TModel>(colName);
				column.AggregationMethod = AggregationMethods.SUM;
				Columns.Add(column);
			}

			return this;
		}
		public BaseSelect<TModel> Min<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var value = ExpressionExtensions.GetAttributeValue<TJoin, object>(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TJoin>(colName);
				column.AggregationMethod = AggregationMethods.MIN;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Min(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TModel>(colName);
				column.AggregationMethod = AggregationMethods.MIN;
				Columns.Add(column);
			}
			return this;
		}

		public BaseSelect<TModel> Max<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = ExpressionExtensions.GetAttributeValue<TJoin, object>(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TJoin>(colName);
				column.AggregationMethod = AggregationMethods.MAX;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Max(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TModel>(colName);
				column.AggregationMethod = AggregationMethods.MAX;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Count<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = ExpressionExtensions.GetAttributeValue<TJoin, object>(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TJoin>(colName);
				column.AggregationMethod = AggregationMethods.COUNT;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Count(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TModel>(colName);
				column.AggregationMethod = AggregationMethods.COUNT;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Avg<TJoin>(Expression<Func<TJoin, object>> expression)
		{
			var colName = expression.GetMemberInfo();

			var value = ExpressionExtensions.GetAttributeValue<TJoin, object>(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TJoin>(colName);
				column.AggregationMethod = AggregationMethods.AVG;
				Columns.Add(column);
			}
			return this;
		}
		public BaseSelect<TModel> Avg(Expression<Func<TModel, object>> expression)
		{
			var colName = expression.GetMemberInfo();
			var value = expression.GetMemberType(colName);

			if (isValidNumericValue(value))
			{
				var column = GenerateSelectColumn<TModel>(colName);
				column.AggregationMethod = AggregationMethods.AVG;
				Columns.Add(column);
			}
			return this;
		}
		#endregion



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