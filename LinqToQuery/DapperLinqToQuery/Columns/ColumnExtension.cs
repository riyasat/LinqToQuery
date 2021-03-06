//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Text.RegularExpressions;
using DapperLinqToQuery.Utilities.Enums;

namespace DapperLinqToQuery.Columns
{
	internal static class ColumnExtension
	{

		internal static string GetSelect(this Column type)
		{
			var column = $" {type.TableAlias}.{type.ColumnName}";
			switch (type.AggregationMethod)
			{
				case AggregationMethods.SUM:
					column = $"SUM({type.TableAlias}.{type.ColumnName})";
					break;
				case AggregationMethods.MIN:
					column = $"MIN({type.TableAlias}.{type.ColumnName})";
					break;
				case AggregationMethods.MAX:
					column = $"MAX({type.TableAlias}.{type.ColumnName})";
					break;
				case AggregationMethods.AVG:
					column = $"AVG({type.TableAlias}.{type.ColumnName})";
					break;
				case AggregationMethods.COUNT:
					column = $"COUNT({type.TableAlias}.{type.ColumnName})";
					break;
			}
			if (type.HasDistinctBy)
			{
				column = $" distinct({column})";
			}

			if (string.IsNullOrWhiteSpace(type.ColumnAlias) == false)
			{
				column = $" {column} AS {type.ColumnAlias}";
			}
			
			return column;
		}

		internal static string GetJoin(this Column type)
		{
			if (type.JoinType == JoinTypes.NONE) return "";

			if (type.JoinWithColumn == null) throw new Exception("Join is not correct");
			string joinSyntax = Regex.Replace(type.JoinType.ToString(), "(\\B[A-Z])", " $1").ToUpper();

			return $" {joinSyntax} {type.TableName} AS {type.TableAlias}" +
					$" ON  {type.JoinWithColumn.TableAlias}.{type.JoinWithColumn.ColumnName} = {type.TableAlias}.{type.ColumnName}";
		}

		internal static string GetWhere(this Column type)
		{
			return $" {type.GetOperator()} {type.Operator}";
		}
		internal static string GetOrderBy(this Column type)
		{
			var tableName = string.IsNullOrWhiteSpace(type.TableAlias)
				? type.TableName
				: type.TableAlias;
			return $" {tableName}.{type.ColumnName} {type.OrderBy.ToString()}";
		}
		internal static string GetSetValue(this Column type)
		{
			var tableName = string.IsNullOrWhiteSpace(type.TableAlias)
				? type.TableName
				: type.TableAlias;
			return $" {tableName}.{type.ColumnName} = @UP{type.ColumnName}";
		}
		private static string GetOperator(this Column type)
		{
			var tableName = string.IsNullOrWhiteSpace(type.TableAlias)
								? type.TableName
								: type.TableAlias;

			switch (type.Condition)
			{
				case ConditionTypes.CONTAINS:
					return $"{tableName}.{type.ColumnName} LIKE '%'+@{type.ColumnName}+'%'";
				case ConditionTypes.NOTCONTAINS:
					return $"{tableName}.{type.ColumnName} NOT LIKE '%'+@{type.ColumnName}+'%'";
				case ConditionTypes.NOTEQUALSTO:
					return $"{tableName}.{type.ColumnName} != @{type.ColumnName}";
				case ConditionTypes.GREATERTHEN:
					return $"{tableName}.{type.ColumnName} > @{type.ColumnName}";
				case ConditionTypes.GREATERTHENEQUALSTO:
					return $"{tableName}.{type.ColumnName} >= @{type.ColumnName}";
				case ConditionTypes.LESSTHEN:
					return $"{tableName}.{type.ColumnName} < @{type.ColumnName}";
				case ConditionTypes.LESSTHENEQUALSTO:
					return $"{tableName}.{type.ColumnName} <= @{type.ColumnName}";
				case ConditionTypes.IN:
					string[] inValues = type.Value as string[];
					var inVariableName = "";
					if (inValues != null)
					{
						for (int i = 0; i < inValues.Length; i++)
						{
							if (string.IsNullOrWhiteSpace(inVariableName) == false)
							{
								inVariableName += ",";
							}
							inVariableName += $"@{type.ColumnName}{i}";
						}
						return $"{tableName}.{type.ColumnName} IN ({inVariableName})";
					}
					return "";
				case ConditionTypes.NOTIN:
					string[] notInValues = type.Value as string[];
					var variableName = "";
					if (notInValues != null)
					{
						for (int i = 0; i < notInValues.Length; i++)
						{
							if (string.IsNullOrWhiteSpace(variableName) == false)
							{
								variableName += ",";
							}
							variableName += $"@{type.ColumnName}{i}";
						}
						return $"{tableName}.{type.ColumnName} NOT IN ({variableName})";
					}
					return "";
				default:
					return $"{tableName}.{type.ColumnName} = @{type.ColumnName}";
			}
		}
	}
}
